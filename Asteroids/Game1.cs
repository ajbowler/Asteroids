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

        Camera camera;
        Skybox skybox;
        Spaceship spaceship;
        List<Torpedo> torpedoes;

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

            Mouse.SetPosition(0, 0);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            device = graphics.GraphicsDevice;
            camera = new Camera(device);
            collisionEngine = new CollisionEngine();
            skybox = new Skybox();
            skybox.LoadModel(this.Content, effect);
            spaceship = new Spaceship();
            spaceship.LoadModel(this.Content, effect);
            torpedoes = new List<Torpedo>();
            torpedoes.Add(new Torpedo());
            torpedoes[0].LoadModel(this.Content, effect);
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

            spaceship.Update(collisionEngine, gameTime);
            camera.Update(spaceship);
            foreach (Torpedo torp in torpedoes)
            {
                torp.Update(gameTime);
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
            torpedoes[0].Draw(this.Content, camera.getView(), camera.getProjection());
            base.Draw(gameTime);
        }
    }
}