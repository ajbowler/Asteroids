using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Asteroids
{
    class Asteroid
    {
        public const string TEXTURE_PATH = "Models/asteroid_texture";

        // Can be 1, 2, or 3
        private int size;
        private bool destroyed;
        private Model model;
        private Texture2D texture;
        private Matrix rotation;
        private Matrix world;
        private Vector3 velocity;
        private Vector3 position;

        public Asteroid(int size, Vector3 position, Model model)
        {
            this.size = size;
            this.destroyed = false;
            this.model = model;
            this.rotation = Matrix.Identity;
            this.world = Matrix.Identity;
            this.velocity = Vector3.Zero;
            this.position = position;
            this.texture = null;
        }

        public void Update()
        {
            // TODO
        }

        public void Draw(ContentManager content, Matrix view, Matrix projection)
        {
            this.texture = content.Load<Texture2D>(TEXTURE_PATH);

            setWorldMatrix(Matrix.CreateScale(20f) * Matrix.CreateTranslation(getPosition()));
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

        public int getSize()
        {
            return this.size;
        }

        public void setSize(int size)
        {
            this.size = size;
        }

        public Model getModel()
        {
            return this.model;
        }

        public void setModel(Model model)
        {
            this.model = model;
        }

        public bool isDestroyed()
        {
            return this.destroyed;
        }

        public void setDestroyed(bool destroyed)
        {
            this.destroyed = destroyed;
        }

        public Matrix getRotation()
        {
            return this.rotation;
        }

        public void setRotation(Matrix rotation)
        {
            this.rotation = rotation;
        }

        public Vector3 getVelocity()
        {
            return this.velocity;
        }

        public void setVelocity(Vector3 velocity)
        {
            this.velocity = velocity;
        }

        public Vector3 getPosition()
        {
            return this.position;
        }

        public void setPosition(Vector3 position)
        {
            this.position = position;
        }

        public Matrix getWorldMatrix()
        {
            return this.world;
        }

        public void setWorldMatrix(Matrix worldMatrix)
        {
            this.world = worldMatrix;
        }
    }
}
