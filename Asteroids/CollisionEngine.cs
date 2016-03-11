using Microsoft.Xna.Framework;
using System;

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

        public bool CollideTwoObjects(SoundEngine soundEngine, BoundingSphere bs1, BoundingSphere bs2)
        {
            if (bs1.Intersects(bs2))
            {
                soundEngine.Explosion().Play();
                return true;
            }
            else
                return false;
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

        public float getEdgeOfUniverse()
        {
            return EDGE_OF_UNIVERSE;
        }
    }
}
