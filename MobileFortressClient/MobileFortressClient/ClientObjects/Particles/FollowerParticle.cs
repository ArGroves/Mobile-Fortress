using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MobileFortressClient.Physics;
using Microsoft.Xna.Framework;

namespace MobileFortressClient.Particles
{
    class FollowerParticle : Particle
    {
        public PhysicsObj Parent { get; set; }
        public Vector3 Offset { get; set; }
        public FollowerParticle(ushort spriteID, PhysicsObj parent, Vector3 positionOffset, Vector3 velocityOffset, float size)
            : base(spriteID, parent.Position+positionOffset, Quaternion.Identity, velocityOffset, size)
        {
            Parent = parent;
            Offset = positionOffset;
            Position = Parent.Entity.Position + Offset;
        }
        public override void Update(float dt)
        {
            Offset += Velocity * dt;
            Position = Parent.Position + Offset;
            if (Position.Y < -25) Sector.Redria.ClientObjects.Remove(this);
        }
    }
}
