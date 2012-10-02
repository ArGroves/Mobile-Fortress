using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MobileFortressClient.Physics;
using Lidgren.Network;
using MobileFortressClient.Particles;
using MobileFortressClient.ClientObjects;

namespace MobileFortressClient.Managers
{
    class EffectManager
    {
        public List<ClientObject> table = new List<ClientObject>();
        List<ClientObject> adding = new List<ClientObject>();
        List<ClientObject> removing = new List<ClientObject>();
        public void Process(float dt)
        {
            foreach (ClientObject particle in adding)
            {
                table.Add(particle);
            }
            adding = new List<ClientObject>(5);
            foreach (ClientObject particle in table)
            {
                particle.Update(dt);
            }
            foreach (ClientObject particle in removing)
            {
                table.Remove(particle);
            }
            removing = new List<ClientObject>(5);
        }
        public void Add(ClientObject obj)
        {
            adding.Add(obj);
        }
        public void Remove(ClientObject obj)
        {
            removing.Add(obj);
        }
    }
}
