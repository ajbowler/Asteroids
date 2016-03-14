using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids
{
    class Powerup
    {
        public enum PowerupType
        {
            Shield,
            Shrink
        };

        public PowerupType Type { get; set; }
        public Vector3 Position { get; set; }
        public bool Activated { get; set; }
        public Texture2D Texture { get; set; }
        public Model Model { get; set; }

        public Powerup(PowerupType type, Vector3 position)
        {
            this.Type = type;
            this.Position = position;
            this.Activated = false;
            this.Texture = null; // TODO add textures and models
            this.Model = null;
        }
    }
}
