using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids
{
    class Torpedo
    {
        public const string MODEL_PATH = "Models/torpedo";
        public const string TEXTURE_PATH = "Models/torp_texture";
        public const float VELOCITY_CONST = 1000f;

        private Model model;
        private Texture2D texture;
        private Vector3 position;
        private Vector3 direction;
        private Matrix world;
        private BoundingSphere boundingSphere;

        public Torpedo(Vector3 pos, Vector3 dir)
        {
            this.model = null;
            this.texture = null;
            this.position = pos;
            this.direction = dir;
            this.world = Matrix.Identity;
        }

        public void LoadModel(ContentManager content, BasicEffect effect)
        {
            float radius = 0f;
            this.model = content.Load<Model>(MODEL_PATH);
            foreach (ModelMesh mesh in model.Meshes)
            {
                radius = Math.Max(radius, mesh.BoundingSphere.Radius);

                foreach (BasicEffect currentEffect in mesh.Effects)
                {
                    this.texture = currentEffect.Texture;
                }

                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = effect.Clone();
                }
            }

            this.boundingSphere = new BoundingSphere(getPosition(), radius);
        }

        public void Update(GameTime gameTime)
        {
            float speed = VELOCITY_CONST / gameTime.ElapsedGameTime.Milliseconds;
            Vector3 velocity = speed * getDirection();
            Vector3 newPos = getPosition() + velocity;
            setPosition(newPos);
        }

        public void Draw(ContentManager content, Matrix view, Matrix projection)
        {
            setWorldMatrix(Matrix.CreateTranslation(getPosition()));
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

        public Model getModel()
        {
            return this.model;
        }

        public Vector3 getPosition()
        {
            return this.position;
        }

        public void setPosition(Vector3 position)
        {
            this.position = position;
        }

        public Vector3 getDirection()
        {
            return this.direction;
        }

        public void setDirection(Vector3 direction)
        {
            this.direction = direction;
        }

        public Matrix getWorldMatrix()
        {
            return this.world;
        }

        public void setWorldMatrix(Matrix world)
        {
            this.world = world;
        }

        public BoundingSphere getBoundingSphere()
        {
            return this.boundingSphere;
        }

        public void setBoundingSphere(BoundingSphere boundingSphere)
        {
            this.boundingSphere = boundingSphere;
        }
    }
}
