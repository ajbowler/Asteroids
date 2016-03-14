using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Asteroids
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        GraphicsDevice device;
        SpriteBatch spriteBatch;
        BasicEffect effect;
        Effect billboardEffect;
        CollisionEngine collisionEngine;
        ParticleEngine particleEngine;
        SoundEngine soundEngine;
        Random rng = new Random();

        MouseState originalMouseState;
        Camera camera;
        Skybox skybox;
        Spaceship spaceship;
        List<Torpedo> torpedoes;
        List<Asteroid> asteroids;
        List<Powerup> powerups;
        Texture2D lifeTexture;
        Texture2D explosionParticleTexture;
        SpriteFont timeFont;
        string gameClock;
        public const int TORPEDO_FIRE_INTERVAL = 2;
        public const int ASTEROID_COUNT = 50;
        public const int POWERUP_SPAWN_INTERVAL = 25;
        public const float AST_ROT_SPEED_LIMIT = 0.05f;
        public const float AST_ROT_MIN_MAGNITUDE = 0f;
        public const float AST_ROT_MAX_MAGNITUDE = 1.0f;
        public const float AST_SPEED_LIMIT = 200f;
        public const float AST_SPAWN_LIMIT = 13000f;
        public const float SHIP_SAFE_SPAWN_ZONE = 2000f;

        public string[] asteroidModelPaths = new string[6] 
        {
            "Models/asteroid_1",
            "Models/asteroid_2",
            "Models/asteroid_3",
            "Models/asteroid_4",
            "Models/asteroid_5",
            "Models/asteroid_6"
        };
        public Model[] asteroidModels;
        public float[] asteroidBSRadius;
        float reloadTimer = 0;
        float powerupTimer = POWERUP_SPAWN_INTERVAL;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            effect = new BasicEffect(GraphicsDevice);
            effect.LightingEnabled = true;

            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.IsFullScreen = true;
            graphics.ApplyChanges();
            Window.Title = "Asteroids 3D";

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            device = graphics.GraphicsDevice;
            Mouse.SetPosition(device.Viewport.Width / 2, device.Viewport.Height / 2);
            originalMouseState = Mouse.GetState();
            billboardEffect = this.Content.Load<Effect>("Shaders/billboard_effect");
            camera = new Camera(device);
            lifeTexture = this.Content.Load<Texture2D>("Sprites/spaceship_sprite");
            explosionParticleTexture = this.Content.Load<Texture2D>("Sprites/explosion_particle");
            timeFont = Content.Load<SpriteFont>("Fonts/Courier New");
            collisionEngine = new CollisionEngine();
            particleEngine = new ParticleEngine(explosionParticleTexture, billboardEffect, device);
            soundEngine = new SoundEngine(this.Content);
            skybox = new Skybox();
            skybox.LoadModel(this.Content, effect);
            spaceship = new Spaceship();
            spaceship.LoadModelAndTexture(this.Content, effect);
            torpedoes = new List<Torpedo>();
            powerups = new List<Powerup>();
            LoadAsteroids();
        }

        protected override void UnloadContent()
        {
            camera = null;
            spaceship = null;
            skybox = null;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            int totalTime = (int) gameTime.TotalGameTime.TotalSeconds;
            gameClock = totalTime.ToString();

            powerupTimer -= (float) gameTime.ElapsedGameTime.TotalSeconds;
            if (powerupTimer <= 0)
            {
                AddPowerup(rng);
                powerupTimer = POWERUP_SPAWN_INTERVAL;
            }

            reloadTimer -= (float) gameTime.ElapsedGameTime.TotalSeconds;
            if (reloadTimer < 0)
                reloadTimer = 0;
            Vector3 direction = camera.GetDirection();

            particleEngine.Update(gameTime);

            if (spaceship != null && spaceship.Lives > 0)
            {
                spaceship.Update(direction, collisionEngine, soundEngine, 
                    particleEngine, asteroids, originalMouseState, gameTime, device);
                camera.Update(spaceship);
                ProcessClick(gameTime);
            }
            else
                spaceship = null;

            for (int i = 0; i < torpedoes.Count; i++)
            {
                if (!(torpedoes[i].Destroyed))
                    torpedoes[i].Update(collisionEngine, soundEngine, particleEngine,
                        gameTime, asteroids);
                else
                    torpedoes.RemoveAt(i);
            }

            for (int i = 0; i < asteroids.Count; i++)
            {
                if (!(asteroids[i].Destroyed))
                    asteroids[i].Update(collisionEngine, soundEngine, gameTime, torpedoes, 
                        asteroids, rng, asteroidModels, asteroidBSRadius);
                else
                    asteroids.RemoveAt(i);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            effect.Projection = camera.Projection;
            effect.View = camera.View;
            skybox.Draw(device, camera.View, camera.Projection);
            particleEngine.Draw(camera);
            foreach (Torpedo torp in torpedoes)
                torp.Draw(this.Content, camera.View, camera.Projection);
            foreach (Asteroid asteroid in asteroids)
                asteroid.Draw(this.Content, camera.View, camera.Projection);
            foreach (Powerup powerup in powerups)
                powerup.Draw(camera.View, camera.Projection);
            if (spaceship != null)
            {
                spaceship.Draw(this.Content, camera.View, camera.Projection);
                DrawLives(spaceship.Lives);
            }
            DrawTime(gameClock);
            base.Draw(gameTime);
        }

        private void FireTorpedo()
        {
            // Do not fire until the 2 second interval has passed
            if (reloadTimer == 0)
            {
                Torpedo torp = new Torpedo(spaceship.Position + new Vector3(0, -20, 0), spaceship.World.Up + camera.GetDirection());
                torp.LoadModelAndTexture(this.Content, effect);
                torpedoes.Add(torp);
                soundEngine.WeaponFire.Play();
                reloadTimer = TORPEDO_FIRE_INTERVAL;
            }
        }

        private void ProcessClick(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                FireTorpedo();
            }
        }

        private void LoadAsteroids()
        {
            asteroids = new List<Asteroid>();

            asteroidModels = LoadAsteroidModelsAndBoundingSpheres();

            for (int i = 0; i < ASTEROID_COUNT; i++)
            {
                int size = rng.Next(0, 6);
                Vector3 position = GenerateRandomPosition();
                BoundingSphere boundingSphere = new BoundingSphere(position, asteroidBSRadius[size]);
                float speed = RandomFloat(-AST_SPEED_LIMIT, AST_SPEED_LIMIT);
                Vector3 direction = RandomUnitVector();

                float yaw = RandomFloat(AST_ROT_MIN_MAGNITUDE, AST_ROT_MAX_MAGNITUDE);
                float pitch = RandomFloat(AST_ROT_MIN_MAGNITUDE, AST_ROT_MAX_MAGNITUDE);
                float roll = RandomFloat(AST_ROT_MIN_MAGNITUDE, AST_ROT_MAX_MAGNITUDE);
                Vector3 ypr = new Vector3(yaw, pitch, roll);
                float rotationSpeed = RandomFloat(-AST_ROT_SPEED_LIMIT, AST_ROT_SPEED_LIMIT);

                asteroids.Add(
                    new Asteroid(size, i, position, speed, direction, ypr,
                    rotationSpeed, asteroidModels[size], boundingSphere));
            }
        }

        private Model[] LoadAsteroidModelsAndBoundingSpheres()
        {
            asteroidModels = new Model[6];
            asteroidBSRadius = new float[6];
            for (int i = 0; i < 6; i++)
            {
                float radius = 0f;
                asteroidModels[i] = Content.Load<Model>(asteroidModelPaths[i]);
                foreach (ModelMesh mesh in asteroidModels[i].Meshes)
                {
                    radius = Math.Max(radius, mesh.BoundingSphere.Radius);

                    foreach (ModelMeshPart meshPart in mesh.MeshParts)
                        meshPart.Effect = effect.Clone();
                }
                asteroidBSRadius[i] = radius;
            }

            return asteroidModels;
        }

        private float RandomFloat(float min, float max)
        {
            return (float)rng.NextDouble() * (max - min) + min;
        }

        private Vector3 RandomUnitVector()
        {
            float x = (float)rng.NextDouble();
            float y = (float)rng.NextDouble();
            float z = (float)rng.NextDouble();
            return new Vector3(x, y, z);
        }

        /**
         * Quick and dirty way of getting a random position 
         * far enough away from the ship's starting position.
         */
        private Vector3 GenerateRandomPosition()
        {
            Vector3 position = Vector3.Zero;
            while (true)
            {
                position.X = RandomFloat(-AST_SPAWN_LIMIT, AST_SPAWN_LIMIT);
                if (Math.Abs(position.X) > SHIP_SAFE_SPAWN_ZONE)
                    break;
            }
            while (true)
            {
                position.Y = RandomFloat(-AST_SPAWN_LIMIT, AST_SPAWN_LIMIT);
                if (Math.Abs(position.Y) > SHIP_SAFE_SPAWN_ZONE)
                    break;
            }
            while (true)
            {
                position.Z = RandomFloat(-AST_SPAWN_LIMIT, AST_SPAWN_LIMIT);
                if (Math.Abs(position.Z) > SHIP_SAFE_SPAWN_ZONE)
                    break;
            }
            return position;
        }

        /**
         * Draws the current number of lives.
         */
        private void DrawLives(int lives)
        {
            int textureWidth = lifeTexture.Width;
            int textureHeight = lifeTexture.Height;
            for (int i = lives - 1; i >= 0; i--)
            {
                Rectangle rect = new Rectangle(textureWidth * i, 0, textureWidth, textureHeight);
                spriteBatch.Begin();
                spriteBatch.Draw(lifeTexture, rect, Color.White);
                spriteBatch.End();
            }
        }

        private void DrawTime(string gameClock)
        {
            string time = gameClock.ToString();
            spriteBatch.Begin();
            Vector2 position = new Vector2(device.Viewport.Width - timeFont.MeasureString(time).X, 0);
            spriteBatch.DrawString(timeFont, time, position, Color.Gold);
            spriteBatch.End();
        }

        private void AddPowerup(Random rng)
        {
            int type = rng.Next();
            Vector3 position = GenerateRandomPosition();
            Powerup powerup;
            if (type % 2 == 0)
                powerup = new Powerup(Powerup.PowerupType.Shield, position);
            else
                powerup = new Powerup(Powerup.PowerupType.Shrink, position);
            powerup.LoadModel(this.Content, effect);
            powerups.Add(powerup);
        }
    }
}
