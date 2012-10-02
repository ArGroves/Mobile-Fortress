using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;

namespace MobileFortressServer.Messages
{
    struct StatusMessage
    {
        public StatusMessage(NetOutgoingMessage msg, float health, ushort[] ammo)
        {
            msg.Write((byte)NetMsgType.ShipUpdate);
            msg.Write(health);
            msg.Write((byte)ammo.Length);
            for (byte i = 0; i < ammo.Length; i++)
            {
                msg.Write(ammo[i]);
            }
        }
    }
}
