using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BEPUphysics.Entities;
using Microsoft.Xna.Framework;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.CollisionShapes;
using BEPUphysics.CollisionShapes.ConvexShapes;

namespace MobileFortressServer
{
    class Resources
    {
        public const int ShipPartIndex = 0;
        public static Entity GetEntity(Vector3 position, UInt16 index)
        {
            switch (index)
            {
                case 0: //Terrain
                    return new Box(position, 750, 5, 750);
                case 1: //Building
                    return new Box(position, 15, 8, 15);
                case 2: //Core
                    Entity shape = new CompoundBody(
                        new List<CompoundShapeEntry>
                        {
                            new CompoundShapeEntry(new BoxShape(2f, 2f, 3f),float.MaxValue),
                            new CompoundShapeEntry(new BoxShape(9f, 0.1f, 2f),new Vector3(0,0,0))
                        }, 50
                        );
                    shape.Position = position;
                    return shape;
                case 3: //Nose
                    return new Box(position+new Vector3(0,0,-0.75f), 2f, 2f, 1.5f);
                case 4: //Engine
                    return new Box(position+new Vector3(0, 0, 0.75f), 2f, 2f, 1.5f);
                default:
                    return new Box(position, 1, 1, 1);
            }
        }
    }
}
