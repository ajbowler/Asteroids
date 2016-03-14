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
        public GraphicsDevice Device { get; set; }
        public Texture2D Texture { get; set; }
        public Effect Effect { get; set; }
        public int Lifetime { get; set; }
        public Vector2 Size { get; set; }
        public Color Color { get; set; }
        public Matrix World { get; set; }
        public Vector3 Position { get; set; }
        public VertexBuffer VertexBuffer { get; set; }
        public IndexBuffer IndexBuffer { get; set; }
        private VertexPositionTexture[] Particle;
        private int[] Indices;

        public ExplosionBillboard(GraphicsDevice device, Texture2D texture, Effect effect, 
            int lifetime, Vector2 size, Vector3 position)
        {
            this.Device = device;
            this.Texture = texture;
            this.Effect = effect;
            this.Lifetime = lifetime;
            this.Size = size;
            this.Position = position;
            this.World = Matrix.Identity;
            this.Particle = new VertexPositionTexture[4];
            this.Indices = new int[6];
            MakeParticle();
        }

        public void Update(GameTime gameTime)
        {
            // TODO
        }

        public void Draw(Camera camera)
        {
            this.Device.SetVertexBuffer(this.VertexBuffer);
            this.Device.Indices = this.IndexBuffer;

            this.Effect.Parameters["ParticleTexture"].SetValue(this.Texture);
            this.Effect.Parameters["View"].SetValue(camera.View);
            this.Effect.Parameters["Projection"].SetValue(camera.Projection);
            this.Effect.Parameters["Size"].SetValue(this.Size);
            this.Effect.Parameters["Up"].SetValue(camera.GetUp());
            this.Effect.Parameters["Side"].SetValue(camera.GetRight());
            this.Effect.Parameters["AlphaTest"].SetValue(true);
            this.Effect.Parameters["AlphaTestGreater"].SetValue(true);
            this.Effect.CurrentTechnique.Passes[0].Apply();

            this.Device.BlendState = BlendState.AlphaBlend;
            this.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 2);
            this.Device.BlendState = BlendState.Opaque;
            this.Device.SetVertexBuffer(null);
            this.Device.Indices = null;
        }

        public void MakeParticle()
        {
            this.Particle[0] = new VertexPositionTexture(this.Position, new Vector2(0, 0));
            this.Particle[1] = new VertexPositionTexture(this.Position, new Vector2(0, 1));
            this.Particle[2] = new VertexPositionTexture(this.Position, new Vector2(1, 1));
            this.Particle[3] = new VertexPositionTexture(this.Position, new Vector2(1, 0));

            this.Indices[0] = 0;
            this.Indices[1] = 3;
            this.Indices[2] = 2;
            this.Indices[3] = 2;
            this.Indices[4] = 1;
            this.Indices[5] = 0;

            this.VertexBuffer = new VertexBuffer(this.Device, typeof(VertexPositionTexture), 
                4, BufferUsage.WriteOnly);
            this.VertexBuffer.SetData<VertexPositionTexture>(this.Particle);
            this.IndexBuffer = new IndexBuffer(this.Device, IndexElementSize.ThirtyTwoBits, 
                6, BufferUsage.WriteOnly);
            this.IndexBuffer.SetData<int>(this.Indices);
        }

        public void DecreaseLifeTime()
        {
            this.Lifetime--;
        }
    }
}
