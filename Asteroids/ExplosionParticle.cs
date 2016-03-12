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
        private float size;
        private Color color;
        private Matrix world;
        private Vector3 position;

        public ExplosionParticle(ContentManager content, int lifetime, float size, Vector3 position)
        {
            this.texture = content.Load<Texture2D>(TEXTURE_PATH);
            this.lifetime = lifetime;
            this.size = size;
            this.position = position;
            this.world = Matrix.Identity;
        }

        public void Update(GameTime gameTime)
        {
            // TODO
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            
        }

        public int getLifeTime()
        {
            return this.lifetime;
        }

        public void DecreaseLifeTime()
        {
            this.lifetime--;
        }

        public float getSize()
        {
            return this.size;
        }

        public void setSize(float size)
        {
            this.size = size;
        }

        public Color getColor()
        {
            return this.color;
        }

        public void setColor(Color color)
        {
            this.color = color;
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

        public void setWorldMatrix(Matrix world)
        {
            this.world = world;
        }
    }
}
