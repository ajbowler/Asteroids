using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace Asteroids
{
    class Spaceship
    {
        public const string MODEL_PATH = "Models/star-wars-vader-tie-fighter";

        Vector3 position;
        Quaternion rotation;
        Matrix world;
        Model model;

        public Spaceship()
        {
            position = new Vector3();
            rotation = Quaternion.Identity;
            model = null;
            Matrix world = Matrix.Identity;
        }

        public void LoadModel(ContentManager content, BasicEffect effect)
        {
            this.model = content.Load<Model>(MODEL_PATH);
            foreach (ModelMesh mesh in model.Meshes)
            {
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

        public void Draw(SpriteBatch spriteBatch, Matrix view, Matrix projection)
        {
            setWorldMatrix(Matrix.CreateScale(0.05f) *
                Matrix.CreateRotationY(MathHelper.Pi) *
                Matrix.CreateFromQuaternion(getRotation()) *
                Matrix.CreateTranslation(getPosition()));

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
                    effect.DirectionalLight0.DiffuseColor = new Vector3(0.0f, 0.0f, 0.0f);
                    effect.DirectionalLight0.Direction = new Vector3(1, 0, 0);
                    effect.DirectionalLight0.SpecularColor = new Vector3(0.0f, 0.0f, 0.0f);
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
