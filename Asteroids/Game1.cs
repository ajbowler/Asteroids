using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Asteroids
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        GraphicsDevice device;
        SpriteBatch spriteBatch;
        BasicEffect effect;
        Matrix viewMatrix, projectionMatrix;

        Spaceship spaceship;

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
            LoadCamera();
            spaceship = new Spaceship();
            spaceship.LoadModel(this.Content, effect);
        }

        private void LoadCamera()
        {
            viewMatrix = Matrix.CreateLookAt(
                new Vector3(0, 20, 50),
                new Vector3(0, 0, 0),
                new Vector3(0, 1, 0)
            );

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
                device.Viewport.AspectRatio,
                1.0f,
                1000.0f
            );
        }

        protected override void UnloadContent()
        {

        }

        private void ProcessKeyboard(GameTime gameTime)
        {
            float leftRightRot = 0;

            float turningSpeed = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;
            turningSpeed *= 1.6f;
            KeyboardState keys = Keyboard.GetState();
            if (keys.IsKeyDown(Keys.Right))
                leftRightRot += turningSpeed;
            if (keys.IsKeyDown(Keys.Left))
                leftRightRot -= turningSpeed;
            float upDownRot = 0;
            if (keys.IsKeyDown(Keys.Down))
                upDownRot += turningSpeed;
            if (keys.IsKeyDown(Keys.Up))
                upDownRot -= turningSpeed;
            Quaternion additionalRot = Quaternion.CreateFromAxisAngle(new Vector3(0, 0, -1), leftRightRot) * 
                Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), upDownRot);
            spaceship.setRotation(spaceship.getRotation() * additionalRot);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            ProcessKeyboard(gameTime);
            spaceship.Update(gameTime);
            UpdateCamera();
            base.Update(gameTime);
        }

        private void UpdateCamera()
        {
            Vector3 newPosition = new Vector3(0, 10.0f, 40.0f);
            newPosition = Vector3.Transform(newPosition, Matrix.CreateFromQuaternion(spaceship.getRotation()));
            newPosition += spaceship.getPosition();

            Vector3 newUp = new Vector3(0, 1, 0);
            newUp = Vector3.Transform(newUp, Matrix.CreateFromQuaternion(spaceship.getRotation()));

            viewMatrix = Matrix.CreateLookAt(newPosition, spaceship.getPosition(), newUp);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, device.Viewport.AspectRatio, 1.0f, 1000.0f);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            effect.Projection = projectionMatrix;
            effect.View = viewMatrix;
            spaceship.Draw(spriteBatch, viewMatrix, projectionMatrix);

            base.Draw(gameTime);
        }
    }
}