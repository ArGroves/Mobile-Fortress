using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Lidgren.Network;
using MobileFortressServer.Data;
using BEPUphysics;
using MobileFortressServer.Ships;

namespace MobileFortressServer.Physics
{
    class Bullet
    {
        public Vector3 Position;
        public Quaternion Orientation;
        public Vector3 Velocity;
        public BulletData Data;
        float Gravity { get { return 4.5f; } }
        float Damping { get { return 0.001f; } }

        public Bullet(Vector3 position, Quaternion orientation, Vector3 velocity, BulletData data)
        {
            Position = position;
            Orientation = orientation;
            Velocity = velocity;
            Data = data;
            Sector.Redria.Bullets.Add(this);
        }
        public void Update(float dt)
        {
            Position += Velocity * dt;
            Velocity -= new Vector3(0, Gravity * dt, 0);
            Velocity *= (1f - Damping);

            Vector3 direction = Velocity;
            direction.Normalize();
            Ray ray = new Ray(Position, direction);//Vector3.Transform(Vector3.Forward, Orientation));
            RayCastResult result;
            Sector.Redria.Space.RayCast(ray, 2, out result);
            if (result.HitObject != null)
            {
                if (result.HitObject.Tag is ShipObj)
                {
                    var ship = (ShipObj)result.HitObject.Tag;
                    var magnitude = Velocity.Length();
                    //var angle = Vector3.Dot(result.HitData.Normal,Velocity)/magnitude;
                    float velocity = magnitude;//(float)(magnitude*angle);
                    ship.BulletStrike(Data,velocity);
                }
                Sector.Redria.Bullets.Remove(this);
            }
            if (Position.Y < -5) Sector.Redria.Bullets.Remove(this);
        }
    }
}
