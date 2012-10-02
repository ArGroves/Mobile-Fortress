using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BEPUphysics.Entities.Prefabs;
using Microsoft.Xna.Framework;

namespace MobileFortressServer.Physics
{
    class Ground : PhysicsObj
    {
        public Ground(Vector3 position)
            : base(new Box(position,750,5,750), 0)
        {

        }
    }
}
