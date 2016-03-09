using Microsoft.Xna.Framework;
using System;

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

        public bool CollidesWithEdge(Vector3 position, BoundingSphere boundingSphere)
        {
            float radius = boundingSphere.Radius;
            return (
                Math.Abs(position.X) + radius > EDGE_OF_UNIVERSE ||
                Math.Abs(position.Y) + radius > EDGE_OF_UNIVERSE ||
                Math.Abs(position.Z) + radius > EDGE_OF_UNIVERSE
            );
        }
    }
}
