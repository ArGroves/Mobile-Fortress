using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MobileFortressClient.Data;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using BEPUphysics;
using MobileFortressClient.Physics;

namespace MobileFortressClient.Particles
{
    class PBullet : Particle
    {
        public BulletData Data;

        public PBullet(Vector3 position, Quaternion orientation, Vector3 velocity, BulletData data)
            : base(data.SpriteID,position,orientation,velocity,data.BulletSize)
        {
            Data = data;
        }
        public override void Update(float dt)
        {
            base.Update(dt);
            Vector3 direction = Velocity;
            direction.Normalize();
            Ray ray = new Ray(Position, direction);//Vector3.Transform(Vector3.Forward, Orientation));
            RayCastResult result;
            Sector.Redria.Space.RayCast(ray, 2, out result);
            if (result.HitObject != null)
            {
                new PExplosion(Position, Data.BulletSize * 4f);
                SoundEffectInstance sound = Resources.Sounds.BulletHit().CreateInstance();
                UpdateSource();
                sound.Apply3D(Camera.Audio, Audio);
                sound.Play();
                Sector.Redria.ClientObjects.Remove(this);
            }
        }
    }
}
