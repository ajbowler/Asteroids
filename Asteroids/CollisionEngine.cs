using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids
{
    class CollisionEngine
    {
        // The max distance of the skybox is 30K, but this eliminates 
        // any overlap between the skybox and the models
        public const float EDGE_OF_UNIVERSE = 29000f;

        public CollisionEngine()
        {

        }

        public BoundingBox LoadBoundingBox(Model model)
        {
            Vector3 modelMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            Vector3 modelMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

            foreach (ModelMesh mesh in model.Meshes)
            {
                Vector3 meshMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);
                Vector3 meshMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    int stride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
                    byte[] vertexData = new byte[stride * meshPart.NumVertices];
                    meshPart.VertexBuffer.GetData(meshPart.VertexOffset * stride, vertexData, 0, meshPart.NumVertices, 1); // fixed 13/4/11
                    Vector3 vertPosition = new Vector3();
                    for (int i = 0; i < vertexData.Length; i += stride)
                    {
                        vertPosition.X = BitConverter.ToSingle(vertexData, i);
                        vertPosition.Y = BitConverter.ToSingle(vertexData, i + sizeof(float));
                        vertPosition.Z = BitConverter.ToSingle(vertexData, i + sizeof(float) * 2);
                        meshMin = Vector3.Min(meshMin, vertPosition);
                        meshMax = Vector3.Max(meshMax, vertPosition);
                    }
                }

                Matrix[] transformation = new Matrix[model.Bones.Count];
                model.CopyAbsoluteBoneTransformsTo(transformation);
                meshMin = Vector3.Transform(meshMin, transformation[mesh.ParentBone.Index]);
                meshMax = Vector3.Transform(meshMax, transformation[mesh.ParentBone.Index]);
                modelMin = Vector3.Min(modelMin, meshMin);
                modelMax = Vector3.Max(modelMax, meshMax);
            }
            return new BoundingBox(modelMin, modelMax);
        }

        public bool HitEdgeOfUniverse(BoundingBox boundingBox)
        {
            Vector3[] corners = boundingBox.GetCorners();
            foreach (Vector3 corner in corners)
            {
                float x = corner.X;
                float y = corner.Y;
                float z = corner.Z;
                if (x >= Math.Abs(EDGE_OF_UNIVERSE) ||
                    y >= Math.Abs(EDGE_OF_UNIVERSE) ||
                    z >= Math.Abs(EDGE_OF_UNIVERSE))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
