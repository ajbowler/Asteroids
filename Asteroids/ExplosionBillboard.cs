using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Asteroids
{
    class ExplosionBillboard
    {
        public GraphicsDevice Device { get; set; }
        public Texture2D Texture { get; set; }
        public Effect Effect { get; set; }
        public float BirthTime { get; set; }
        public float LifeTime { get; set; }
        public bool Dead { get; set; }
        public Vector2 Size { get; set; }
        public Vector3 Position { get; set; }
        public VertexBuffer VertexBuffer { get; set; }
        public IndexBuffer IndexBuffer { get; set; }
        private VertexPositionTexture[] Particle;
        private int[] Indices;

        public ExplosionBillboard(GraphicsDevice device, Texture2D texture, Effect effect, 
            GameTime gameTime, Vector2 size, Vector3 position)
        {
            this.Device = device;
            this.Texture = texture;
            this.Effect = effect;
            this.BirthTime = (float) gameTime.TotalGameTime.TotalMilliseconds;
            this.LifeTime = 3000f; // 3 seconds
            this.Dead = false;
            this.Size = size;
            this.Position = position;
            this.Particle = new VertexPositionTexture[4];
            this.Indices = new int[6];
            MakeParticle();
        }

        public void Update(GameTime gameTime)
        {
            float totalTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
            if (totalTime - this.BirthTime > this.LifeTime)
            {
                this.Dead = true;
            }
        }

        public void Draw(Camera camera)
        {
            if (this.Dead)
                return;

            this.Device.SetVertexBuffer(this.VertexBuffer);
            this.Device.Indices = this.IndexBuffer;

            this.Effect.Parameters["ParticleTexture"].SetValue(this.Texture);
            this.Effect.Parameters["View"].SetValue(camera.View);
            this.Effect.Parameters["Projection"].SetValue(camera.Projection);
            this.Effect.Parameters["Size"].SetValue(this.Size);
            this.Effect.Parameters["Up"].SetValue(camera.GetUp());
            this.Effect.Parameters["Side"].SetValue(camera.GetRight());
            this.Effect.CurrentTechnique.Passes[0].Apply();

            this.Device.BlendState = BlendState.AlphaBlend;
            this.Device.DepthStencilState = DepthStencilState.DepthRead;
            this.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 2);
            this.Device.BlendState = BlendState.Opaque;
            this.Device.DepthStencilState = DepthStencilState.Default;
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
    }
}
