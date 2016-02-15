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
        Model spaceshipModel;

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
            spaceshipModel = LoadSpaceShipModel();
        }

        private Model LoadSpaceShipModel()
        {
            Model model = Content.Load<Model>("Models/star-wars-vader-tie-fighter");
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = effect.Clone();
                }
            }
            return model;
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

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            effect.Projection = projectionMatrix;
            effect.View = viewMatrix;
            DrawSpaceshipModel();

            base.Draw(gameTime);
        }

        private void DrawSpaceshipModel()
        {
            Matrix world = Matrix.CreateScale(0.05f) * 
                Matrix.CreateRotationY(MathHelper.Pi) * 
                Matrix.CreateTranslation(new Vector3(0, 0, 0));

            Matrix[] spaceshipTransformation = new Matrix[spaceshipModel.Bones.Count];
            spaceshipModel.CopyAbsoluteBoneTransformsTo(spaceshipTransformation);
            foreach (ModelMesh mesh in spaceshipModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = world;
                    effect.View = viewMatrix;
                    effect.Projection = projectionMatrix;
                    effect.EnableDefaultLighting();
                    effect.DirectionalLight0.DiffuseColor = new Vector3(0.0f, 0.0f, 0.0f);
                    effect.DirectionalLight0.Direction = new Vector3(1, 0, 0);
                    effect.DirectionalLight0.SpecularColor = new Vector3(0.0f, 0.0f, 0.0f);
                }
                mesh.Draw();
            }
        }
    }
}