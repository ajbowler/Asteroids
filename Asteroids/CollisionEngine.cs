using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;

namespace Asteroids
{
    class CollisionEngine
    {
        // The max distance of the skybox is 15K, but this eliminates 
        // any overlap between the skybox and the models
        public float EDGE_OF_UNIVERSE = 14000;

        public CollisionEngine()
        {

        }

        public bool CollideTwoObjects(SoundEngine soundEngine, BoundingSphere bs1, BoundingSphere bs2)
        {
            if (bs1.Intersects(bs2))
            {
                if (soundEngine.Explosion.State != SoundState.Playing)
                    soundEngine.Explosion.Play();
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
    }
}
