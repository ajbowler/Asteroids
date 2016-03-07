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

        public bool HitEdgeOfUniverse(BoundingSphere boundingSphere)
        {
            return false; // TODO
        }
    }
}
