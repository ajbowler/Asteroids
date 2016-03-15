using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Asteroids
{
    class SoundEngine
    {
        public SoundEffectInstance ShipEngine;
        public SoundEffectInstance WeaponFire;
        public SoundEffectInstance Explosion;
        public SoundEffectInstance Shield;
        public SoundEffectInstance Grow;
        public SoundEffectInstance Shrink;

        public SoundEngine(ContentManager content)
        {
            LoadSounds(content);
        }

        private void LoadSounds(ContentManager content)
        {
            SoundEffect e1 = content.Load<SoundEffect>("Sounds/ship_engine");
            SoundEffect e2 = content.Load<SoundEffect>("Sounds/fire_weapon");
            SoundEffect e3 = content.Load<SoundEffect>("Sounds/explosion");
            SoundEffect e4 = content.Load<SoundEffect>("Sounds/shield");
            SoundEffect e5 = content.Load<SoundEffect>("Sounds/grow");
            SoundEffect e6 = content.Load<SoundEffect>("Sounds/shrink");
            this.ShipEngine = e1.CreateInstance();
            this.ShipEngine.IsLooped = true;
            this.WeaponFire = e2.CreateInstance();
            this.Explosion = e3.CreateInstance();
            this.Shield = e4.CreateInstance();
            this.Grow = e5.CreateInstance();
            this.Shrink = e6.CreateInstance();
        }
    }
}
