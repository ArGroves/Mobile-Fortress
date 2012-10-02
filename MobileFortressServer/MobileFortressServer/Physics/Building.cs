using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.Entities;
using Microsoft.Xna.Framework;

namespace MobileFortressServer.Physics
{
    class Building : PhysicsObj
    {
        public int Height;
        public Building(Vector3 position, int height)
            : base()
        {
            Entity box = new Box(new Vector3(position.X, position.Y+16*height, position.Z), 30, 16*height, 30);
            SetEntity(box, 1);
        }
    }
}
