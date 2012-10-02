using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MobileFortressServer.Physics;
using Lidgren.Network;

namespace MobileFortressServer.Managers
{
    class BulletManager
    {
        List<Bullet> table = new List<Bullet>();
        List<Bullet> adding = new List<Bullet>();
        List<Bullet> removing = new List<Bullet>();
        public void Process(float dt)
        {
            foreach (Bullet particle in adding)
            {
                //particle.ID = (ushort)table.Count;
                table.Add(particle);
            }
            adding = new List<Bullet>(5);
            foreach (Bullet particle in table)
            {
                particle.Update(dt);
            }
            foreach (Bullet particle in removing)
            {
                /*for (int i = particle.ID + 1; i < table.Count; i++ )
                {
                    table[i].ID--;
                }*/
                table.Remove(particle);
            }
            removing = new List<Bullet>(5);
        }
        public void Add(Bullet obj)
        {
            adding.Add(obj);
        }
        public void Remove(Bullet obj)
        {
            removing.Add(obj);
        }
        public Bullet Retrieve(int index)
        {
            return table[index];
        }
    }
}
