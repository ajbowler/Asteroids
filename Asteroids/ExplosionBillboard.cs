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
    class ExplosionBillboard
    {
        public Texture2D Texture { get; set; }
        public int Lifetime { get; set; }
        public float Size { get; set; }
        public Color Color { get; set; }
        public Matrix World { get; set; }
        public Vector3 Position { get; set; }
        public VertexBuffer VertexBuffer { get; set; }
        public IndexBuffer IndexBuffer { get; set; }
        private VertexPositionTexture[] Particle;

        public ExplosionBillboard(Texture2D texture, int lifetime, float size, Vector3 position)
        {
            this.Texture = texture;
            this.Lifetime = lifetime;
            this.Size = size;
            this.Position = position;
            this.World = Matrix.Identity;
            this.Particle = new VertexPositionTexture[4];
        }

        public void Update(GameTime gameTime)
        {
            // TODO
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            // TODO
        }

        public void MakeParticle()
        {
            // TODO
        }

        public void DecreaseLifeTime()
        {
            this.Lifetime--;
        }
    }
}
