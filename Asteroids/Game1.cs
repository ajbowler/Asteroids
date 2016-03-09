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

        MouseState originalMouseState;
        Camera camera;
        Skybox skybox;
        Spaceship spaceship;
        List<Torpedo> torpedoes;
        public const int TORPEDO_FIRE_INTERVAL = 2;
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
            spaceship.LoadModel(this.Content, effect);
            torpedoes = new List<Torpedo>();
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

            List<int> destroyedTorpedoes = new List<int>();

            for (int i = 0; i < torpedoes.Count; i++)
            {
                if (!(torpedoes[i].isDestroyed()))
                    torpedoes[i].Update(collisionEngine, gameTime);
                else
                    torpedoes.RemoveAt(i);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            effect.Projection = camera.getProjection();
            effect.View = camera.getView();
            skybox.Draw(device, camera.getView(), camera.getProjection());
            spaceship.Draw(this.Content, camera.getView(), camera.getProjection());
            foreach (Torpedo torp in torpedoes)
                torp.Draw(this.Content, camera.getView(), camera.getProjection());
            base.Draw(gameTime);
        }

        private void FireTorpedo()
        {
            // Do not fire until the 2 second interval has passed
            if (timer == 0)
            {
                Torpedo torp = new Torpedo(spaceship.getPosition(), spaceship.getWorldMatrix().Up + camera.getDirection());
                torp.LoadModel(this.Content, effect);
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
    }
}