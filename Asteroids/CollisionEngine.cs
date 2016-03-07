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

        public bool ShipCollidesWithEdge(Spaceship spaceship)
        {
            Vector3 position = spaceship.getPosition();
            float radius = spaceship.getBoundingSphere().Radius;
            return (
                Math.Abs(position.X) + radius > EDGE_OF_UNIVERSE ||
                Math.Abs(position.Y) + radius > EDGE_OF_UNIVERSE ||
                Math.Abs(position.Z) + radius > EDGE_OF_UNIVERSE
            );
        }
    }
}
