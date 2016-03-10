using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Asteroids
{
    class Asteroid
    {
        // Can be 1, 2, or 3
        private int size;
        private bool destroyed;
        private Model model;
        private Matrix rotation;
        private Vector3 velocity;
        private Vector3 position;

        public Asteroid(int size, Vector3 position)
        {
            this.size = size;
            this.destroyed = false;
            this.model = null;
            this.rotation = Matrix.Identity;
            this.velocity = Vector3.Zero;
            this.position = position;
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
    }
}
