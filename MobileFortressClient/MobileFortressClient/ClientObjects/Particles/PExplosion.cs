using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MobileFortressClient.Physics;

namespace MobileFortressClient.Particles
{
    class PExplosion : Particle
    {
        protected override float Gravity { get { return 0; } }

        float expandedSize = 5f;
        bool expanding = true;
        public PExplosion(Vector3 position, float size)
            : base(2,position,Quaternion.Identity,Vector3.Zero,0.2f)
        {
            expandedSize = size;
        }
        public override void Update(float dt)
        {
            if (expanding)
                if (Size < expandedSize)
                    Size += 12f * dt;
                else
                    expanding = false;
            else
            {
                Size -= 25f * dt;
                if (Size <= 0.2f)
                    Sector.Redria.ClientObjects.Remove(this);
            }
            base.Update(dt);
        }
    }
}
