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
        CollisionEngine collisionEngine;
        Random rng = new Random();

        MouseState originalMouseState;
        Camera camera;
        Skybox skybox;
        Spaceship spaceship;
        List<Torpedo> torpedoes;
        List<Asteroid> asteroids;
        public const int TORPEDO_FIRE_INTERVAL = 2;
        public const float AST_ROT_SPEED_LIMIT = 0.05f;
        public const float AST_ROT_MIN_MAGNITUDE = 0f;
        public const float AST_ROT_MAX_MAGNITUDE = 1.0f;
        public const float AST_SPEED_LIMIT = 50f;
        public const float AST_SPAWN_LIMIT = 13000f;
        public const float SHIP_SAFE_SPAWN_ZONE = 2000f;
        public const int ASTEROID_COUNT = 50;
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
        float timer = 0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            effect = new BasicEffect(GraphicsDevice);
            effect.LightingEnabled = true;

            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
            graphics.IsFullScreen = false;
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
            camera = new Camera(device);
            collisionEngine = new CollisionEngine();
            skybox = new Skybox();
            skybox.LoadModel(this.Content, effect);
            spaceship = new Spaceship();
            spaceship.LoadModelAndTexture(this.Content, effect);
            torpedoes = new List<Torpedo>();
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

            timer -= (float) gameTime.ElapsedGameTime.TotalSeconds;
            if (timer < 0)
                timer = 0;
            Vector3 direction = camera.getDirection();
            spaceship.Update(direction, collisionEngine, originalMouseState, gameTime, device);
            camera.Update(spaceship);

            ProcessClick(gameTime);

            for (int i = 0; i < torpedoes.Count; i++)
            {
                if (!(torpedoes[i].isDestroyed()))
                    torpedoes[i].Update(collisionEngine, gameTime, asteroids);
                else
                    torpedoes.RemoveAt(i);
            }

            for (int i = 0; i < asteroids.Count; i++)
            {
                if (!(asteroids[i].isDestroyed()))
                    asteroids[i].Update(collisionEngine, gameTime, torpedoes, rng, 
                        asteroidModels, asteroidBSRadius);
                else
                    asteroids.RemoveAt(i);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            effect.Projection = camera.getProjection();
            effect.View = camera.getView();
            skybox.Draw(device, camera.getView(), camera.getProjection());
            foreach (Torpedo torp in torpedoes)
                torp.Draw(this.Content, camera.getView(), camera.getProjection());
            foreach (Asteroid asteroid in asteroids)
                asteroid.Draw(this.Content, camera.getView(), camera.getProjection());
            spaceship.Draw(this.Content, camera.getView(), camera.getProjection());
            base.Draw(gameTime);
        }

        private void FireTorpedo()
        {
            // Do not fire until the 2 second interval has passed
            if (timer == 0)
            {
                Torpedo torp = new Torpedo(spaceship.getPosition(), spaceship.getWorldMatrix().Up + camera.getDirection());
                torp.LoadModelAndTexture(this.Content, effect);
                torpedoes.Add(torp);
                timer = TORPEDO_FIRE_INTERVAL;
            }
        }

        private void ProcessClick(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed)
                FireTorpedo();
        }

        private void LoadAsteroids()
        {
            asteroids = new List<Asteroid>();

            asteroidModels = LoadAsteroidModelsAndBoundingSpheres();

            for (int i = 0; i < ASTEROID_COUNT; i++)
            {
                int size = rng.Next(0, 6);
                Vector3 position = GenerateRandomAsteroidPosition();
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
         * Quick and dirty way of getting a random asteroid position 
         * far enough away from the ship's starting position.
         */
        private Vector3 GenerateRandomAsteroidPosition()
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
    }
}
