using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MobileFortressClient.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace MobileFortressClient.MobileObjects
{
    class Rocket : MobileObj
    {
        SoundEffectInstance rocketEngineSound;
        AudioEmitter Audio = new AudioEmitter();
        bool smoke = false;
        public Rocket(MobileFortressClient game, ushort resource, Vector3 position, Quaternion orientation)
            : base(game, resource, position, orientation)
        {
            rocketEngineSound = Resources.Sounds.RocketEngine.CreateInstance();
            UpdateAudio();
            SoundEffectInstance rocketNoise = Resources.Sounds.WeaponSounds[3].CreateInstance();
            rocketNoise.Apply3D(Camera.Audio,Audio);
            rocketNoise.Play();

            rocketEngineSound.IsLooped = true;
            rocketEngineSound.Apply3D(Camera.Audio, Audio);
            rocketEngineSound.Play();
        }
        public override void Update(float dt)
        {
            base.Update(dt);
            UpdateAudio();
            rocketEngineSound.Apply3D(Camera.Audio, Audio);
            smoke = !smoke;
            if (smoke)
            {
                float x = ((float)Particle.pRandomizer.NextDouble() - .5f)*2.5f;
                float y = ((float)Particle.pRandomizer.NextDouble() - .5f)*2.5f;
                Vector3 vel = Vector3.Transform(new Vector3(x, y, 4), Orientation);
                new PRocketSmoke(this, worldTransform.Backward * 1.5f, vel, 1f);
            }
        }
        void UpdateAudio()
        {
            Audio.Position = Entity.Position / Resources.AudioPositionQuotient;
            Audio.Forward = Entity.WorldTransform.Forward;
            Audio.Up = Entity.WorldTransform.Up;
            Audio.Velocity = Entity.LinearVelocity;
        }
        public override void Destroy()
        {
            rocketEngineSound.Stop();
            base.Destroy();
        }
    }
}
