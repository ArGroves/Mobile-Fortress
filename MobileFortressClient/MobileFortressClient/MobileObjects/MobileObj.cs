using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MobileFortressClient.Physics;
using Microsoft.Xna.Framework;

namespace MobileFortressClient.MobileObjects
{
    class MobileObj : PhysicsObj
    {
        public MobileObj(MobileFortressClient game, ushort resource, Vector3 position, Quaternion orientation)
            : base(game, position, orientation, resource)
        {
        }
        public MobileObj(MobileFortressClient game, ushort resource, Vector3 position, Quaternion orientation, Vector3 velocity)
            : base(game, position, orientation, resource)
        {
            Velocity = velocity;
        }
    }
}
