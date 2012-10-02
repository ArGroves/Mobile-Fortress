using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MobileFortressClient.Physics;
using BEPUphysics.Collidables;

namespace MobileFortressClient.Managers
{
    class TerrainManager
    {
        public float[,] Island { get { return Terrain.Map; } }
        public Heightmap Terrain;
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
                table.Remove(obj);
            }
            removing = new List<PhysicsObj>();
        }
        public void Add(PhysicsObj obj)
        {
            adding.Add(obj);
        }
        public void Remove(PhysicsObj obj)
        {
            removing.Add(obj);
        }
    }
}
