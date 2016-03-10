using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Asteroids
{
    class CollisionEngine
    {
        // The max distance of the skybox is 15K, but this eliminates 
        // any overlap between the skybox and the models
        public const float EDGE_OF_UNIVERSE = 14000;

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

        public bool TorpedoCollidesWithAsteroid(Torpedo torpedo, Asteroid asteroid)
        {
            return torpedo.getBoundingSphere().Intersects(asteroid.getBoundingSphere());
        }

        public float getEdgeOfUniverse()
        {
            return EDGE_OF_UNIVERSE;
        }
    }
}
