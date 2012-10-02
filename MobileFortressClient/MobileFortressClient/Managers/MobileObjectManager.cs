using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MobileFortressClient.Physics;

namespace MobileFortressClient.Managers
{
    class MobileObjectManager
    {
        public List<PhysicsObj> table = new List<PhysicsObj>();
        List<PhysicsObj> adding = new List<PhysicsObj>();
        List<PhysicsObj> removing = new List<PhysicsObj>();
        public void Process(float dt)
        {
            foreach (PhysicsObj particle in adding)
            {
                table.Add(particle);
            }
            adding = new List<PhysicsObj>(5);
            foreach (PhysicsObj particle in table)
            {
                particle.Update(dt);
            }
            foreach (PhysicsObj particle in removing)
            {
                table.Remove(particle);
            }
            removing = new List<PhysicsObj>(5);
        }
        public void Add(PhysicsObj obj)
        {
            adding.Add(obj);
        }
        public void Remove(PhysicsObj obj)
        {
            removing.Add(obj);
        }
        public PhysicsObj Retrieve(UInt16 ID)
        {
            foreach (PhysicsObj ship in table)
            {
                if (ship.ID == ID) return ship;
            }
            return null;
        }
    }
}
