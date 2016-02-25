using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace Asteroids
{
    class Spaceship
    {
        public const string MODEL_PATH = "Models/spaceship";
        public const string TEXTURE_PATH = "Models/metal";

        private Vector3 position;
        private Quaternion rotation;
        private Matrix world;
        private Model model;
        private Texture2D texture;

        public Spaceship()
        {
            position = new Vector3();
            rotation = Quaternion.Identity;
            model = null;
            this.texture = null;
            Matrix world = Matrix.Identity;
        }

        public void LoadModel(ContentManager content, BasicEffect effect)
        {
            this.model = content.Load<Model>(MODEL_PATH);
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect currentEffect in mesh.Effects)
                {
                    this.texture = currentEffect.Texture;
                }

                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = effect.Clone();
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            float moveSpeed = gameTime.ElapsedGameTime.Milliseconds / 50.0f;
            MoveForward(moveSpeed);
        }

        public void Draw(ContentManager content, Matrix view, Matrix projection)
        {
            setWorldMatrix(Matrix.CreateRotationX(MathHelper.Pi * 3 / 2) *
                Matrix.CreateFromQuaternion(getRotation()) *
                Matrix.CreateTranslation(getPosition()));

            this.texture = content.Load<Texture2D>(TEXTURE_PATH);

            Matrix[] transformation = new Matrix[this.model.Bones.Count];
            this.model.CopyAbsoluteBoneTransformsTo(transformation);
            foreach (ModelMesh mesh in this.model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;
                    effect.EnableDefaultLighting();
                    effect.TextureEnabled = true;
                    effect.Texture = this.texture;
                }
                mesh.Draw();
            }
        }

        public void MoveForward(float speed)
        {
            Vector3 addVector = Vector3.Transform(new Vector3(0, 0, -1), getRotation());
            setPosition(this.position + (addVector * speed));
        }

        public Vector3 getPosition()
        {
            return this.position;
        }

        public void setPosition(Vector3 position)
        {
            this.position = position;
        }

        public Quaternion getRotation()
        {
            return this.rotation;
        }

        public void setRotation(Quaternion rotation)
        {
            this.rotation = rotation;
        }

        public Matrix getWorldMatrix()
        {
            return this.world;
        }

        public void setWorldMatrix(Matrix world)
        {
            this.world = world;
        }
    }
}
