using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MobileFortressClient.Physics
{
    class Building : PhysicsObj
    {
        public Building(Vector3 position, ushort resource)
            : base(MobileFortressClient.Game)
        {
            Resource = Resources.GetResource(resource);
            Entity = Resources.GetEntity(position, 1);
            Game.Components.Add(this);
            Sector.Redria.Add(this);
        }
    }
}
