using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids
{
    class SoundEngine
    {
        public SoundEffectInstance ShipEngine;
        public SoundEffectInstance WeaponFire;
        public SoundEffectInstance Explosion;

        public SoundEngine(ContentManager content)
        {
            LoadSounds(content);
        }

        private void LoadSounds(ContentManager content)
        {
            SoundEffect e1 = content.Load<SoundEffect>("Sounds/ship_engine");
            SoundEffect e2 = content.Load<SoundEffect>("Sounds/fire_weapon");
            SoundEffect e3 = content.Load<SoundEffect>("Sounds/explosion");
            this.ShipEngine = e1.CreateInstance();
            this.ShipEngine.IsLooped = true;
            this.WeaponFire = e2.CreateInstance();
            this.Explosion = e3.CreateInstance();
        }
    }
}
