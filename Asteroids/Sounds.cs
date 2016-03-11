using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids
{
    class Sounds
    {
        private SoundEffectInstance shipEngine;
        private SoundEffectInstance weaponFire;
        private SoundEffectInstance explosion;

        public Sounds(ContentManager content)
        {
            LoadSounds(content);
        }

        private void LoadSounds(ContentManager content)
        {
            SoundEffect e1 = content.Load<SoundEffect>("Sounds/ship_engine");
            SoundEffect e2 = content.Load<SoundEffect>("Sounds/fire_weapon");
            SoundEffect e3 = content.Load<SoundEffect>("Sounds/explosion");
            this.shipEngine = e1.CreateInstance();
            this.shipEngine.IsLooped = true;
            this.weaponFire = e2.CreateInstance();
            this.explosion = e3.CreateInstance();
        }

        public SoundEffectInstance ShipEngine()
        {
            return this.shipEngine;
        }

        public SoundEffectInstance FireWeapon()
        {
            return this.weaponFire;
        }

        public SoundEffectInstance Explosion()
        {
            return this.explosion;
        }
    }
}
