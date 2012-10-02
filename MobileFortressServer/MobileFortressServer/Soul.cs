using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MobileFortressServer.Ships;
using MobileFortressServer.Physics;
using Lidgren.Network;
using MobileFortressServer.Character;

namespace MobileFortressServer
{
    class Soul
    {
        public NetConnection Owner;
        public ShipObj currentShip;
        public CharacterController currentCharacter;
        public Sector currentSector;
        public Soul(Lidgren.Network.NetConnection owner)
        {
            this.Owner = owner;
        }
    }
}
