using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MobileFortressClient.Physics;

namespace MobileFortressClient.Particles
{
    class PGroundSmoke : Particle
    {
        PhysicsObj Parent;
        Vector3 Offset;
        float expansion = 0.05f;
        float maxLifetime = 2f;
        float lifetime = 2f;
        protected override float Gravity { get { return 0f; } }
        protected override float Damping { get { return 0f; } }
        public PGroundSmoke(PhysicsObj parent, Vector3 position, Vector3 velocity, float size)
            : base(3, position, Quaternion.Identity, velocity, size)
        {
            Parent = parent;
            Offset = position;
            Position = Parent.Entity.Position+Offset;
        }
        public override void Update(float dt)
        {
            Offset += Velocity*dt;
            Position = new Vector3(Parent.Position.X+Offset.X, 3 + Offset.Y, Parent.Position.Z+Offset.Z);
            lifetime -= dt;
            Size += expansion;
            Alpha = (lifetime / maxLifetime)*.5f;
            if (lifetime <= 0)
                Sector.Redria.ClientObjects.Remove(this);
        }
    }
}
