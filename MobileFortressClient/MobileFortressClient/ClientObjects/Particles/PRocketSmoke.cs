using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MobileFortressClient.Physics;
using MobileFortressClient.Ships;
using MobileFortressClient.MobileObjects;

namespace MobileFortressClient.Particles
{
    class PRocketSmoke : FollowerParticle
    {
        float expansion = -0.02f;
        float maxLifetime = .25f;
        float lifetime = .25f;
        protected override float Gravity { get { return 0f; } }
        protected override float Damping { get { return 0f; } }
        public PRocketSmoke(MobileObj parent, Vector3 position, Vector3 velocity, float size)
            : base(3, parent, position, velocity, size)
        {
        }
        public override void Update(float dt)
        {
            lifetime -= dt;
            Size += expansion;
            Alpha = (lifetime / maxLifetime) * .5f;
            base.Update(dt);
            if (lifetime <= 0)
                Sector.Redria.ClientObjects.Remove(this);
        }
    }
}

