using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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
        public int Size { get; set; }
        public int ID { get; set; }
        public bool Destroyed { get; set; }
        public Model Model { get; set; }
        public Texture2D Texture { get; set; }
        public Vector3 YPR { get; set; }
        public float RotationSpeed { get; set; }
        public float Speed { get; set; }
        public Matrix World { get; set; }
        public Vector3 Direction { get; set; }
        public Vector3 Position { get; set; }
        public BoundingSphere BoundingSphere { get; set; }

        public Asteroid(int size, int id, Vector3 position, float speed, Vector3 direction, 
            Vector3 ypr, float rotationSpeed, Model model, BoundingSphere boundingSphere)
        {
            this.Size = size;
            this.ID = id;
            this.Destroyed = false;
            this.Model = model;
            this.YPR = ypr;
            this.RotationSpeed = rotationSpeed;
            this.World = Matrix.Identity;
            this.Speed = speed;
            this.Direction = direction;
            this.Position = position;
            this.Texture = null;
            this.BoundingSphere = ScaleBoundingSphere(boundingSphere);
        }

        public void Update(CollisionEngine collisionEngine, SoundEngine soundEngine, 
            GameTime gameTime, List<Torpedo> torpedoes, List<Asteroid> asteroids, 
            Random rng, Model[] models, float[] sphereRadius)
        {
            CheckCollisions(collisionEngine, soundEngine, torpedoes, asteroids, rng, models, sphereRadius);

            float yprRate = this.RotationSpeed / gameTime.ElapsedGameTime.Milliseconds;
            Vector3 ypr = UpdateYPR(yprRate);
            this.YPR = ypr;

            float speed = (float) this.Speed / gameTime.ElapsedGameTime.Milliseconds;
            Vector3 velocity = speed * this.Direction;
            UpdatePosition(this.Position + velocity);
        }

        public void Draw(ContentManager content, Matrix view, Matrix projection)
        {
            this.Texture = content.Load<Texture2D>(TEXTURE_PATH);
            Vector3 ypr = this.YPR;

            this.World = Matrix.CreateScale(DetermineScale()) *
                Matrix.CreateRotationX(ypr.X) *
                Matrix.CreateRotationY(ypr.Y) *
                Matrix.CreateRotationZ(ypr.Z) *
                Matrix.CreateTranslation(this.Position);
            Matrix[] transformation = new Matrix[this.Model.Bones.Count];
            this.Model.CopyAbsoluteBoneTransformsTo(transformation);
            foreach (ModelMesh mesh in this.Model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = World;
                    effect.View = view;
                    effect.Projection = projection;
                    effect.EnableDefaultLighting();
                    effect.TextureEnabled = true;
                    effect.Texture = this.Texture;
                }
                mesh.Draw();
            }
        }

        private void CheckCollisions(CollisionEngine collisionEngine, SoundEngine soundEngine, 
            List<Torpedo> torpedoes, List<Asteroid> asteroids, Random rng, Model[] models, 
            float[] sphereRadius)
        {
            // The asteroid bounces off if it collides with the edge of the universe.
            if (collisionEngine.CollidesWithEdge(this.Position, this.BoundingSphere))
            {
                float edge = collisionEngine.EDGE_OF_UNIVERSE;
                Vector3 pos = this.Position;
                Vector3 dir = this.Direction;
                if (Math.Abs(pos.X) > edge)
                    dir.X = -dir.X;
                else if (Math.Abs(pos.Y) > edge)
                    dir.Y = -dir.Y;
                else if (Math.Abs(pos.Z) > edge)
                    dir.Z = -dir.Z;
                this.Direction = dir;
            }

            // Destroy or decrease the size if it hits a torpedo
            foreach (Torpedo torpedo in torpedoes)
            {
                if (collisionEngine.CollideTwoObjects(this.BoundingSphere, 
                    torpedo.BoundingSphere))
                {
                    torpedo.Destroyed = true;
                    soundEngine.Explosion.Play();
                    DecreaseSize(rng, models, sphereRadius);
                    break;
                }
            }

            // Destroy or decrease the size if it hits another asteroid
            foreach (Asteroid asteroid in asteroids)
            {
                if (asteroid.ID != this.ID)
                {
                    if (collisionEngine.CollideTwoObjects(asteroid.BoundingSphere, 
                        this.BoundingSphere))
                    {
                        if (soundEngine.Explosion.State != SoundState.Playing)
                            soundEngine.Explosion.Play();
                        DecreaseSize(rng, models, sphereRadius);
                        Vector3 exitVector = CalculateExitVector(asteroid);
                        this.Direction = exitVector;
                        break;
                    }
                }
            }
        }

        private void DecreaseSize(Random rng, Model[] models, float[] sphereRadius)
        {
            int newSize = this.Size;

            // KABOOM
            if (newSize < 2)
            {
                this.Destroyed = true;
                return;
            }
            else
            {
                newSize -= 2;

                // Reinitialize properties
                this.Model = models[newSize];
                this.Size = newSize;
                BoundingSphere bs = new BoundingSphere(this.Position, sphereRadius[newSize]);
                this.BoundingSphere = ScaleBoundingSphere(bs);
            }
        }

        private Vector3 CalculateExitVector(Asteroid asteroid)
        {
            Vector3 pos1 = this.Position;
            Vector3 pos2 = asteroid.Position;
            Vector3 normalize = pos1 - pos2;
            normalize.Normalize();
            Vector3 dir1 = this.Direction;
            Vector3 dir2 = asteroid.Direction;
            float dot1 = Vector3.Dot(dir1, normalize);
            float dot2 = Vector3.Dot(dir2, normalize);
            float mass1 = (this.Size + 1) / 2;
            float mass2 = (asteroid.Size + 1) / 2;
            float momentum = (2 * (dot1 - dot2)) / (mass1 + mass2);
            Vector3 newDirection = dir1 - momentum * mass2 * normalize;
            return newDirection;
        }

        private Vector3 UpdateYPR(float rate)
        {
            Vector3 ypr = this.YPR;
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
            int size = this.Size;
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
            return new BoundingSphere(this.Position, radius);
        }

        public void UpdatePosition(Vector3 position)
        {
            this.Position = position;
            this.BoundingSphere = new BoundingSphere(position, this.BoundingSphere.Radius);
        }
    }
}
