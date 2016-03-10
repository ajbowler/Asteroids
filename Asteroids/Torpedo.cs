using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

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
        private bool destroyed;

        public Torpedo(Vector3 pos, Vector3 dir)
        {
            this.model = null;
            this.texture = null;
            this.position = pos;
            this.direction = dir;
            this.world = Matrix.Identity;
            this.boundingSphere = new BoundingSphere();
            this.destroyed = false;
        }

        public void LoadModelAndTexture(ContentManager content, BasicEffect effect)
        {
            float radius = 0f;
            this.model = content.Load<Model>(MODEL_PATH);
            foreach (ModelMesh mesh in model.Meshes)
            {
                radius = Math.Max(radius, mesh.BoundingSphere.Radius);

                foreach (BasicEffect currentEffect in mesh.Effects)
                    this.texture = currentEffect.Texture;

                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                    meshPart.Effect = effect.Clone();
            }

            this.boundingSphere = new BoundingSphere(getPosition(), radius);
            this.texture = content.Load<Texture2D>(TEXTURE_PATH);
        }

        public void Update(CollisionEngine collisionEngine, GameTime gameTime)
        {
            CheckCollisions(collisionEngine);
            float speed = VELOCITY_CONST / gameTime.ElapsedGameTime.Milliseconds;
            Vector3 velocity = speed * getDirection();
            Vector3 newPos = getPosition() + velocity;
            setPosition(newPos);
        }

        public void Draw(ContentManager content, Matrix view, Matrix projection)
        {
            if (isDestroyed())
                return;

            setWorldMatrix(Matrix.CreateTranslation(getPosition()));
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

        private void CheckCollisions(CollisionEngine collisionEngine)
        {
            // Destroy the torpedo if it hits the edge of the universe
            if (collisionEngine.CollidesWithEdge(getPosition(), getBoundingSphere()))
                setDestroyed(true);
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
            setBoundingSphere(position);
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

        public void setBoundingSphere(Vector3 position)
        {
            this.boundingSphere = new BoundingSphere(position, getBoundingSphere().Radius);
        }

        public bool isDestroyed()
        {
            return this.destroyed;
        }

        public void setDestroyed(bool destroyed)
        {
            this.destroyed = destroyed;
        }
    }
}
