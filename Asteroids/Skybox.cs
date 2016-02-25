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
    class Skybox
    {
        public const string MODEL_PATH = "Models/skybox";

        private Model model;
        private Texture2D[] textures;

        public Skybox()
        {
            this.model = null;
            this.textures = new Texture2D[6];
        }

        public void LoadModel(ContentManager content, BasicEffect effect)
        {
            Model model = content.Load<Model>(MODEL_PATH);
            int i = 0;
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect currentEffect in mesh.Effects)
                {
                    this.textures[i++] = currentEffect.Texture;
                }

                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    meshPart.Effect = effect.Clone();
                }
            }
            this.model = model;
        }

        public void Draw(GraphicsDevice device, Matrix view, Matrix projection)
        {
            Matrix[] transformation = new Matrix[this.model.Bones.Count];
            this.model.CopyAbsoluteBoneTransformsTo(transformation);
            int i = 0;
            foreach (ModelMesh mesh in this.model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = Matrix.CreateScale(20.0f) * transformation[mesh.ParentBone.Index];
                    effect.View = view;
                    effect.Projection = projection;
                    effect.EnableDefaultLighting();
                    effect.DirectionalLight0.DiffuseColor = new Vector3(0.0f, 0.0f, 0.0f);
                    effect.DirectionalLight0.Direction = new Vector3(1, 0, 0);
                    effect.DirectionalLight0.SpecularColor = new Vector3(0.0f, 0.0f, 0.0f);
                    effect.TextureEnabled = true;
                    effect.Texture = this.textures[i++];
                }
                mesh.Draw();
            }
        }
    }
}
