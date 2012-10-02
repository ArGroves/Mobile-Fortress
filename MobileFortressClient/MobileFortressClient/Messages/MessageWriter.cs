using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MobileFortressClient.Data;
using Microsoft.Xna.Framework;
using Lidgren.Network;

namespace MobileFortressClient.Messages
{
    class MessageWriter
    {
        public static void ShipDataOutputMessage(ShipData src)
        {
            var msg = Network.Client.CreateMessage();
            var Nose = src.NoseID;
            var Core = src.CoreID;
            var Engine = src.EngineID;
            int i = 0;
            var Weapons = new int[src.Weapons.Values.Count];
            var FireGroups = new byte[src.Weapons.Values.Count];
            if(src.Nose.WeaponSlots != null)
                foreach (Vector3 offset in src.Nose.WeaponSlots)
                {
                    if (src.Weapons[offset] != null)
                    {
                        Weapons[i] = src.Weapons[offset].Index;
                        FireGroups[i++] = src.Weapons[offset].fireGroup;
                    }
                }
            if (src.Core.WeaponSlots != null)
                foreach (Vector3 offset in src.Core.WeaponSlots)
                {
                    if (src.Weapons[offset] != null)
                    {
                        Weapons[i] = src.Weapons[offset].Index;
                        FireGroups[i++] = src.Weapons[offset].fireGroup;
                    }
                }
            if (src.Engine.WeaponSlots != null)
                foreach (Vector3 offset in src.Engine.WeaponSlots)
                {
                    if (src.Weapons[offset] != null)
                    {
                        Weapons[i] = src.Weapons[offset].Index;
                        FireGroups[i++] = src.Weapons[offset].fireGroup;
                    }
                }
            msg.Write((byte)NetMsgType.ShipDataOutput);
            msg.Write(Nose);
            msg.Write(Core);
            msg.Write(Engine);

            var NC = src.NoseColor;
            msg.Write(NC.R); msg.Write(NC.G); msg.Write(NC.B);
            var CC = src.CoreColor;
            msg.Write(CC.R); msg.Write(CC.G); msg.Write(CC.B);
            var TC = src.TailColor;
            msg.Write(TC.R); msg.Write(TC.G); msg.Write(TC.B);
            var WC = src.WeaponColor;
            msg.Write(WC.R); msg.Write(WC.G); msg.Write(WC.B);

            msg.Write(Weapons.Length);
            for (i = 0; i < Weapons.Length; i++)
            {
                msg.Write(Weapons[i]);
                msg.Write(FireGroups[i]);
            }
            Network.Client.SendMessage(msg, Lidgren.Network.NetDeliveryMethod.ReliableOrdered);
        }

        public static void ControlMessage(ControlKey key, bool edge)
        {
            var msg = Network.Client.CreateMessage();
            msg.Write((byte)NetMsgType.Control);
            msg.Write((byte)key);
            msg.Write(edge);
            Network.Client.SendMessage(msg, NetDeliveryMethod.ReliableSequenced, 0);
        }

        public static void ControlUpdateMessage(float pitch, float yaw, float roll)
        {
            var msg = Network.Client.CreateMessage();
            msg.Write((byte)NetMsgType.ControlUpdate);
            msg.Write(pitch);
            msg.Write(yaw);
            msg.Write(roll);
            Network.Client.SendMessage(msg, NetDeliveryMethod.ReliableSequenced, 1);
        }
    }
}
