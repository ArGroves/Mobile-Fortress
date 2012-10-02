using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MobileFortressServer.Physics;
using Lidgren.Network;
using MobileFortressServer.Messages;

namespace MobileFortressServer.Managers
{
    class MobileObjectManager
    {
        public List<PhysicsObj> table = new List<PhysicsObj>();
        List<PhysicsObj> adding = new List<PhysicsObj>();
        List<PhysicsObj> removing = new List<PhysicsObj>();

        ushort objectCount = 0;

        public void Process(float dt)
        {
            foreach (PhysicsObj obj in adding)
            {
                table.Add(obj);
            }
            adding = new List<PhysicsObj>(5);
            foreach (PhysicsObj obj in table)
            {
                obj.Update(dt);
            }
            foreach (PhysicsObj obj in removing)
            {
                table.Remove(obj);
            }
            removing = new List<PhysicsObj>(5);
        }
        public void Add(PhysicsObj obj)
        {
            obj.ID = objectCount++;
            adding.Add(obj);
        }
        public void Remove(PhysicsObj obj)
        {
            removing.Add(obj);
        }
        public void Load(NetConnection connection)
        {
            foreach (PhysicsObj ship in table)
            {
                MessageWriter.ClientEntityCreationMessage(connection, NetEntityType.Missile, ship.ID, ship.Position,
                    ship.Orientation, ship.resource_index);
            }
        }

        public void SendUpdate(NetConnection connection)
        {
            var soul = (Soul)connection.Tag;
            foreach (PhysicsObj ship in table)
            {
                MessageWriter.EntityUpdateMessage(connection, ship.ID, NetEntityType.Missile, ship.Position, ship.Orientation, ship.Velocity);
            }
        }
    }
}
