using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MobileFortressServer.Physics;
using System.Collections;
using MobileFortressServer.Ships;
using Lidgren.Network;
using MobileFortressServer.Messages;
using Microsoft.Xna.Framework;

namespace MobileFortressServer.Managers
{
    class ShipManager
    {
        public List<ShipObj> table = new List<ShipObj>();
        List<ShipObj> adding = new List<ShipObj>();
        List<ShipObj> removing = new List<ShipObj>();

        ushort shipCount = 0;

        public void Process(float dt)
        {
            foreach (ShipObj obj in adding)
            {
                table.Add(obj);
            }
            adding = new List<ShipObj>();
            foreach (ShipObj obj in table)
            {
                obj.Update(dt);
            }
            foreach (ShipObj obj in removing)
            {
                table.Remove(obj);
            }
            removing = new List<ShipObj>();
        }
        public void Add(ShipObj obj)
        {
            obj.ID = shipCount++;
            adding.Add(obj);
        }
        public void Remove(ShipObj obj)
        {
            removing.Add(obj);
        }
        public ShipObj Retrieve(int index)
        {
            return table[index];
        }

        public void Load(NetConnection connection)
        {
            foreach (ShipObj ship in table)
            {
                MessageWriter.ClientEntityCreationMessage(connection, ship.ID, ship.Position,
                    ship.Orientation, ship.Data.NoseID,ship.Data.CoreID,ship.Data.EngineID, ship.Data.WeaponIDs, 0);
            }
        }

        public void SendUpdate(NetConnection connection)
        {
            var soul = (Soul)connection.Tag;
            foreach (ShipObj ship in table)
            {
                MessageWriter.EntityUpdateMessage(connection, ship.ID, NetEntityType.Ship, ship.Position, ship.Orientation, ship.Velocity);
            }
        }
    }
}
