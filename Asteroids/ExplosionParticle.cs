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
    class ExplosionParticle
    {
        public const string TEXTURE_PATH = "Sprites/explosion_particle";

        public Texture2D Texture { get; set; }
        public int Lifetime { get; set; }
        private float Size { get; set; }
        private Color Color { get; set; }
        private Matrix World { get; set; }
        private Vector3 Position { get; set; }

        public ExplosionParticle(ContentManager content, int lifetime, float size, Vector3 position)
        {
            this.Texture = content.Load<Texture2D>(TEXTURE_PATH);
            this.Lifetime = lifetime;
            this.Size = size;
            this.Position = position;
            this.World = Matrix.Identity;
        }

        public void Update(GameTime gameTime)
        {
            // TODO
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            // TODO
        }

        public void DecreaseLifeTime()
        {
            this.Lifetime--;
        }
    }
}
