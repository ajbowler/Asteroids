using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Asteroids
{
    class ParticleEngine
    {
        public GraphicsDevice Device { get; set; }
        public Texture2D Texture { get; set; }
        public Effect Effect { get; set; }
        public List<ExplosionBillboard> Particles { get; set; }

        public ParticleEngine(Texture2D texture, Effect effect, GraphicsDevice device)
        {
            this.Device = device;
            this.Texture = texture;
            this.Effect = effect;
            this.Particles = new List<ExplosionBillboard>();
        }

        public void AddParticle(GameTime gameTime, Vector3 position)
        {
            this.Particles.Add(
                new ExplosionBillboard(this.Device, this.Texture, 
                    this.Effect, gameTime, new Vector2(1000f), position)
                );
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < this.Particles.Count; i++)
            {
                this.Particles[i].Update(gameTime);
                if (this.Particles[i].Dead)
                    this.Particles.RemoveAt(i);
            }
        }

        public void Draw(Camera camera)
        {
            foreach (ExplosionBillboard billboard in this.Particles)
                billboard.Draw(camera);
        }
    }
}
