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

        private Texture2D texture;
        private int lifetime;

        public ExplosionParticle(ContentManager content)
        {
            this.texture = content.Load<Texture2D>(TEXTURE_PATH);
        }

        public void Update(GameTime gameTime)
        {
            // TODO
        }

        public void Draw()
        {
            // TODO
        }
    }
}
