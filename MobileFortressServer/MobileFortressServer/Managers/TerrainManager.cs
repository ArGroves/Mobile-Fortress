using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Lidgren.Network;
using MobileFortressServer.Messages;
using Microsoft.Xna.Framework;
using BEPUphysics.PositionUpdating;
using MobileFortressServer.Physics;
using MobileFortressServer.Data;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.MathExtensions;

namespace MobileFortressServer.Managers
{
    class TerrainManager
    {
        Heightmap island;
        List<Rectangle> mods = new List<Rectangle>();

        List<PhysicsObj> table = new List<PhysicsObj>();
        List<PhysicsObj> adding = new List<PhysicsObj>();
        List<PhysicsObj> removing = new List<PhysicsObj>();
        public void Process(float dt)
        {
            foreach (PhysicsObj obj in adding)
            {
                table.Add(obj);
            }
            adding = new List<PhysicsObj>();
            foreach (PhysicsObj obj in table)
            {
                obj.Update(dt);
            }
            foreach (PhysicsObj obj in removing)
            {
                for (int i = (int)obj.ID + 1; i < table.Count; i++)
                {
                    table[i].ID--;
                }
                table.RemoveAt((int)obj.ID);
            }
            removing = new List<PhysicsObj>();
        }
        public void Add(PhysicsObj obj)
        {
            obj.ID = (ushort)(table.Count + adding.Count);
            obj.Entity.PositionUpdateMode = PositionUpdateMode.Passive;
            adding.Add(obj);
        }
        public void Remove(PhysicsObj obj)
        {
            removing.Add(obj);
        }
        public PhysicsObj Retrieve(int index)
        {
            return table[index];
        }

        public void Load(NetConnection connection)
        {
            if (island.Map == null)
                throw new ApplicationException("Island map not generated yet.");
            MessageWriter.MapLoadingMessage(connection, 300, 8, 80f);
            MessageWriter.MapLoadingMessage(connection, mods.ToArray());

            foreach (PhysicsObj terrain in table)
            {
                if (terrain.resource_index == 0) continue;
                MessageWriter.ClientEntityCreationMessage(connection, NetEntityType.Building, terrain.ID, terrain.Position,
                    terrain.Orientation, terrain.resource_index);
            }
        }

        public void Initialize()
        {
            island = new Heightmap(300, 8, 80f);
            int offset = island.Map.GetLength(0)/2;
            var usedPoints = new List<int>();
            for (int i = 0; i < 32; i++)
            {
                int x, y;
                do
                {
                    x = 96 + island.Generator.Next(0, 17) * 4;
                    y = 96 + island.Generator.Next(0, 17) * 4;
                } while (island.Map[x, y] <= 1f || usedPoints.Contains(x+y*island.Map.GetLength(0)));

                usedPoints.Add(x + y * island.Map.GetLength(0));

                int levels = island.Generator.Next(3, 15);
                ushort resource = (ushort)island.Generator.Next(2, 5);
                for (int j = 0; j < levels; j++)
                {
                    new PhysicsObj(new Box(ToPhysicsCoords(x,y) + new Vector3(0,j*16+8,0), 30, 16, 30), resource);
                }
                Rectangle area = new Rectangle(x - 1, y - 1, 2, 2);
                mods.Add(area);
                island.FlattenArea(area);
            }
        }

        Vector3 ToPhysicsCoords(int x, int y)
        {
            Vector3 returnvalue = new Vector3(0,0,0);
            AffineTransform transform = island.Terrain.WorldTransform;
            island.Terrain.Shape.GetPosition(x, y, ref transform, out returnvalue);
            return returnvalue;
        }
    }
}
