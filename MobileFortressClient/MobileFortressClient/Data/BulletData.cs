using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MobileFortressClient.Physics;
using MobileFortressClient.Particles;

namespace MobileFortressClient.Data
{
    class BulletData : ProjectileData
    {
        public ushort SpriteID;
        public float BulletSize;
        public float MuzzleVel;
        public float BulletSpread;
        public float Power;

        public BulletData(ushort sprite, float size, float velocity, float spread, float power)
            : base()
        {
            SpriteID = sprite;
            BulletSize = size;
            MuzzleVel = velocity;
            BulletSpread = spread;
            Power = power;
        }

        public override void Create(Vector3 position, Quaternion orientation, Vector3 velocity)
        {
            new PBullet(position, orientation, velocity, this);
        }
        public override void Create(Vector3 position, Quaternion orientation, Vector3 velocity, BEPUphysics.Entities.Entity target)
        {
            Create(position, orientation, velocity);
        }
        public override ProjectileData Copy()
        {
            return new BulletData(SpriteID, BulletSize, MuzzleVel, BulletSpread, Power);
        }
    }
}
