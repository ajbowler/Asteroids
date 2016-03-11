using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Asteroids
{
    class Asteroid
    {
        public const string TEXTURE_PATH = "Models/asteroid_texture";

        // Can be 0-5
        private int size;
        private int id;
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

        public Asteroid(int size, int id, Vector3 position, float speed, Vector3 direction, 
            Vector3 ypr, float rotationSpeed, Model model, BoundingSphere boundingSphere)
        {
            this.size = size;
            this.id = id;
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

        public void Update(CollisionEngine collisionEngine, SoundEngine soundEngine, 
            GameTime gameTime, List<Torpedo> torpedoes, List<Asteroid> asteroids, 
            Random rng, Model[] models, float[] sphereRadius)
        {
            CheckCollisions(collisionEngine, soundEngine, torpedoes, asteroids, rng, models, sphereRadius);

            float yprRate = getRotationSpeed() / gameTime.ElapsedGameTime.Milliseconds;
            Vector3 ypr = UpdateYPR(yprRate);
            setYPR(ypr);

            float speed = (float) getSpeed() / gameTime.ElapsedGameTime.Milliseconds;
            Vector3 velocity = speed * getDirection();
            setPosition(getPosition() + velocity);
            BoundingSphere bs = new BoundingSphere(getPosition(), getBoundingSphere().Radius);
            setBoundingSphere(bs);
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

        private void CheckCollisions(CollisionEngine collisionEngine, SoundEngine soundEngine, 
            List<Torpedo> torpedoes, List<Asteroid> asteroids, Random rng, Model[] models, 
            float[] sphereRadius)
        {
            // The asteroid bounces off if it collides with the edge of the universe.
            if (collisionEngine.CollidesWithEdge(getPosition(), getBoundingSphere()))
            {
                float edge = collisionEngine.getEdgeOfUniverse();
                Vector3 pos = getPosition();
                Vector3 dir = getDirection();
                if (Math.Abs(pos.X) > edge)
                    dir.X = -dir.X;
                else if (Math.Abs(pos.Y) > edge)
                    dir.Y = -dir.Y;
                else if (Math.Abs(pos.Z) > edge)
                    dir.Z = -dir.Z;
                setDirection(dir);
            }

            // Destroy or decrease the size if it hits a torpedo
            foreach (Torpedo torpedo in torpedoes)
            {
                if (collisionEngine.CollideTwoObjects(soundEngine, getBoundingSphere(), 
                    torpedo.getBoundingSphere()))
                {
                    torpedo.setDestroyed(true);
                    DecreaseSize(rng, models, sphereRadius);
                    break;
                }
            }

            // Destroy or decrease the size if it hits another asteroid
            foreach (Asteroid asteroid in asteroids)
            {
                if (asteroid.getID() != this.getID())
                {
                    if (collisionEngine.CollideTwoObjects(soundEngine, 
                        asteroid.getBoundingSphere(), this.getBoundingSphere()))
                    {
                        DecreaseSize(rng, models, sphereRadius);
                        Vector3 exitVector = CalculateExitVector(asteroid);
                        setDirection(exitVector);
                        break;
                    }
                }
            }
        }

        private void DecreaseSize(Random rng, Model[] models, float[] sphereRadius)
        {
            int newSize = getSize();

            // KABOOM
            if (newSize < 2)
            {
                setDestroyed(true);
                return;
            }
            else
            {
                newSize -= 2;

                // Reinitialize properties
                setModel(models[newSize]);
                setSize(newSize);
                BoundingSphere bs = new BoundingSphere(getPosition(), sphereRadius[newSize]);
                setBoundingSphere(ScaleBoundingSphere(bs));
            }
        }

        private Vector3 CalculateExitVector(Asteroid asteroid)
        {
            Vector3 pos1 = getPosition();
            Vector3 pos2 = asteroid.getPosition();
            Vector3 normalize = pos1 - pos2;
            normalize.Normalize();
            Vector3 dir1 = getDirection();
            Vector3 dir2 = asteroid.getDirection();
            float dot1 = Vector3.Dot(dir1, normalize);
            float dot2 = Vector3.Dot(dir2, normalize);
            float mass1 = (getSize() + 1) / 2;
            float mass2 = (asteroid.getSize() + 1) / 2;
            float momentum = (2 * (dot1 - dot2)) / (mass1 + mass2);
            Vector3 newDirection = dir1 - momentum * mass2 * normalize;
            return newDirection;
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

        public int getID()
        {
            return this.id;
        }

        public void setID(int id)
        {
            this.id = id;
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
