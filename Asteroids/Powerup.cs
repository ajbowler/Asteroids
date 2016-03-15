using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Asteroids
{
    class Powerup
    {
        public enum PowerupType
        {
            Shield,
            Shrink
        };

        public const string SHIELD_MODEL_PATH = "Models/shield";
        public const string SHIELD_TEXTURE_PATH = "Models/shield_texture";
        public const string SHRINK_MODEL_PATH = "Models/shrink";
        public const string SHRINK_TEXTURE_PATH = "Models/shrink_texture";

        public PowerupType Type { get; set; }
        public Vector3 Position { get; set; }
        public float Timer { get; set; }
        public bool Activated { get; set; }
        public bool Collected { get; set; }
        public Model Model { get; set; }
        public Texture2D Texture { get; set; }
        public BoundingSphere BoundingSphere { get; set; }
        public Matrix World { get; set; }

        public Powerup(PowerupType type, Vector3 position)
        {
            this.Type = type;
            this.Position = position;
            this.Model = null;
            this.Activated = false;
            this.Collected = false;
            this.Timer = 10;
            this.BoundingSphere = new BoundingSphere();
            this.World = Matrix.Identity;
        }

        public void LoadModel(ContentManager content, BasicEffect effect)
        {
            float radius = 0f;
            if (this.Type == PowerupType.Shield)
            {
                this.Model = content.Load<Model>(SHIELD_MODEL_PATH);
                this.Texture = content.Load<Texture2D>(SHIELD_TEXTURE_PATH);
            }
            else
            {
                this.Model = content.Load<Model>(SHRINK_MODEL_PATH);
                this.Texture = content.Load<Texture2D>(SHRINK_TEXTURE_PATH);
            }
            
            foreach (ModelMesh mesh in this.Model.Meshes)
            {
                radius = Math.Max(radius, mesh.BoundingSphere.Radius);

                foreach (BasicEffect currentEffect in mesh.Effects)
                    this.Texture = currentEffect.Texture;

                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                    meshPart.Effect = effect.Clone();
            }
            this.BoundingSphere = new BoundingSphere(this.Position, radius * 300f);
        }

        public void Update(GameTime gameTime)
        {
            if (this.Type == PowerupType.Shrink && this.Activated)
            {
                this.Timer -= (float) gameTime.ElapsedGameTime.TotalSeconds;
                if (this.Timer <= 0)
                    this.Activated = false;
            }
        }

        public void Draw(Matrix view, Matrix projection)
        {
            if (this.Collected)
                return;

            this.World = Matrix.CreateScale(300f) * Matrix.CreateTranslation(this.Position);
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
    }
}
