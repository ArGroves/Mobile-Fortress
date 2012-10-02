using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MobileFortressServer.Physics;
using Microsoft.Xna.Framework;

namespace MobileFortressServer.Data
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

        public override void Create(Vector3 position, Quaternion orientation, Vector3 inputVelocity, out Vector3 velocity)
        {
            float x = (float)((ProjectileData.pRandom.NextDouble()-0.5) * BulletSpread);
            float y = (float)((ProjectileData.pRandom.NextDouble()-0.5) * BulletSpread);
            velocity = Vector3.Transform(new Vector3(x, y, -MuzzleVel), orientation) + inputVelocity;
            new Bullet(position, orientation, velocity, this);
        }
        public override void Create(Vector3 position, Quaternion orientation, Vector3 inputVelocity, out Vector3 velocity, PhysicsObj target)
        {
            Create(position, orientation, inputVelocity, out velocity);
        }
        public override ProjectileData Copy()
        {
            return new BulletData(SpriteID, BulletSize, MuzzleVel, BulletSpread, Power);
        }
    }
}
