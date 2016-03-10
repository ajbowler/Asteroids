using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Asteroids
{
    class Asteroid
    {
        public const string TEXTURE_PATH = "Models/asteroid_texture";

        // Can be 0-5
        private int size;
        private bool destroyed;
        private Model model;
        private Texture2D texture;
        private Vector3 ypr;
        private float rotationSpeed;
        private float speed;
        private Matrix world;
        private Vector3 direction;
        private Vector3 position;
        private BoundingSphere boundingSphere;

        public Asteroid(int size, Vector3 position, float speed, Vector3 direction, 
            Vector3 ypr, float rotationSpeed, Model model, BoundingSphere boundingSphere)
        {
            this.size = size;
            this.destroyed = false;
            this.model = model;
            this.ypr = ypr;
            this.rotationSpeed = rotationSpeed;
            this.world = Matrix.Identity;
            this.speed = speed;
            this.direction = direction;
            this.position = position;
            this.texture = null;
            this.boundingSphere = ScaleBoundingSphere(boundingSphere);
        }

        public void Update(GameTime gameTime)
        {
            float yprRate = getRotationSpeed() / gameTime.ElapsedGameTime.Milliseconds;
            Vector3 ypr = UpdateYPR(yprRate);
            setYPR(ypr);

            float speed = (float) getSpeed() / gameTime.ElapsedGameTime.Milliseconds;
            Vector3 velocity = speed * getDirection();
            setPosition(getPosition() + velocity);
        }

        public void Draw(ContentManager content, Matrix view, Matrix projection)
        {
            this.texture = content.Load<Texture2D>(TEXTURE_PATH);
            Vector3 ypr = getYPR();

            setWorldMatrix(Matrix.CreateScale(DetermineScale()) * 
                Matrix.CreateRotationX(ypr.X) *
                Matrix.CreateRotationY(ypr.Y) *
                Matrix.CreateRotationZ(ypr.Z) * 
                Matrix.CreateTranslation(getPosition())
            );
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

        private Vector3 UpdateYPR(float rate)
        {
            Vector3 ypr = getYPR();
            ypr.X += rate;
            ypr.Y += rate;
            ypr.Z += rate;
            return ypr;
        }

        /**
         * This returns a scale based on the model provided. 
         * They're magic numbers but I suck at Blender.
         */
        private float DetermineScale()
        {
            int size = getSize();
            if (size == 0 || size == 1)
                return 200f;
            else if (size == 2)
                return 400f;
            else if (size == 3 || size == 4)
                return 600f;
            else return 500f;
        }


        /**
         * Resets the bounding sphere size after intialization because of 
         * the scaling of the models. I blame Blender again.
         */
        private BoundingSphere ScaleBoundingSphere(BoundingSphere boundingSphere)
        {
            float radius = boundingSphere.Radius;
            radius *= DetermineScale();
            return new BoundingSphere(this.position, radius);
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

        public float getSpeed()
        {
            return this.speed;
        }

        public void setSpeed(float speed)
        {
            this.speed = speed;
        }

        public Vector3 getDirection()
        {
            return this.direction;
        }

        public void setDirection(Vector3 direction)
        {
            this.direction = direction;
        }

        public Vector3 getYPR()
        {
            return this.ypr;
        }

        public void setYPR(Vector3 ypr)
        {
            this.ypr = ypr;
        }

        public float getRotationSpeed()
        {
            return this.rotationSpeed;
        }

        public void setRotationSpeed(float rotationSpeed)
        {
            this.rotationSpeed = rotationSpeed;
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
