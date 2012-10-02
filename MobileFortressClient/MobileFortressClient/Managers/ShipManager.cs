using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MobileFortressClient.Ships;

namespace MobileFortressClient.Managers
{
    class ShipManager
    {
        List<ShipObj> table = new List<ShipObj>();
        List<ShipObj> adding = new List<ShipObj>();
        List<ShipObj> removing = new List<ShipObj>();

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
            adding.Add(obj);
        }
        public void Remove(ShipObj obj)
        {
            removing.Add(obj);
        }
        public ShipObj Retrieve(UInt16 ID)
        {
            foreach (ShipObj ship in table)
            {
                if (ship.ID == ID) return ship;
            }
            return null;
        }
        public List<ShipObj> GetTable()
        {
            return table;
        }
    }
}
