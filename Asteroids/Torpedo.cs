using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;

namespace Asteroids
{
    class Torpedo
    {
        public const string MODEL_PATH = "Models/torpedo";
        public const string TEXTURE_PATH = "Models/torp_texture";
        public const float VELOCITY_CONST = 1000f;

        public Model Model { get; set; }
        public Texture2D Texture { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Direction { get; set; }
        public Matrix World { get; set; }
        public BoundingSphere BoundingSphere { get; set; }
        public bool Destroyed { get; set; }

        public Torpedo(Vector3 pos, Vector3 dir)
        {
            this.Model = null;
            this.Texture = null;
            this.Position = pos;
            this.Direction = dir;
            this.World = Matrix.Identity;
            this.BoundingSphere = new BoundingSphere();
            this.Destroyed = false;
        }

        public void LoadModelAndTexture(ContentManager content, BasicEffect effect)
        {
            float radius = 0f;
            this.Model = content.Load<Model>(MODEL_PATH);
            foreach (ModelMesh mesh in this.Model.Meshes)
            {
                radius = Math.Max(radius, mesh.BoundingSphere.Radius);

                foreach (BasicEffect currentEffect in mesh.Effects)
                    this.Texture = currentEffect.Texture;

                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                    meshPart.Effect = effect.Clone();
            }

            this.BoundingSphere = new BoundingSphere(this.Position, radius);
            this.Texture = content.Load<Texture2D>(TEXTURE_PATH);
        }

        public void Update(CollisionEngine collisionEngine, SoundEngine soundEngine,
            ParticleEngine particleEngine, GameTime gameTime, List<Asteroid> asteroids)
        {
            CheckCollisions(collisionEngine, soundEngine, particleEngine, gameTime, asteroids);
            float speed = VELOCITY_CONST / gameTime.ElapsedGameTime.Milliseconds;
            Vector3 velocity = speed * this.Direction;
            Vector3 newPos = this.Position + velocity;
            UpdatePosition(newPos);
        }

        public void Draw(ContentManager content, Matrix view, Matrix projection)
        {
            if (this.Destroyed)
                return;

            this.World = Matrix.CreateScale(10f) * Matrix.CreateTranslation(this.Position);
            Matrix[] transformation = new Matrix[this.Model.Bones.Count];
            this.Model.CopyAbsoluteBoneTransformsTo(transformation);
            foreach (ModelMesh mesh in this.Model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = this.World;
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
            ParticleEngine particleEngine, GameTime gameTime, List<Asteroid> asteroids)
        {
            // Destroy the torpedo if it hits the edge of the universe
            if (collisionEngine.CollidesWithEdge(this.Position, this.BoundingSphere))
            {
                if (soundEngine.Explosion.State != SoundState.Playing)
                    soundEngine.Explosion.Play();
                this.Destroyed = true;
                particleEngine.AddParticle(gameTime, this.Position);
            }

            // Destroy the torpedo if it hits an asteroid
            foreach (Asteroid asteroid in asteroids)
            {
                if (collisionEngine.CollideTwoObjects(this.BoundingSphere, asteroid.BoundingSphere))
                {
                    particleEngine.AddParticle(gameTime, this.Position);
                    this.Destroyed = true;
                    break;
                }
            }
        }

        public void UpdatePosition(Vector3 position)
        {
            this.Position = position;
            this.BoundingSphere = new BoundingSphere(position, this.BoundingSphere.Radius);
        }
    }
}
