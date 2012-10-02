using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MobileFortressServer.Data;
using Microsoft.Xna.Framework;
using Lidgren.Network;

namespace MobileFortressServer.Messages
{
    struct ShipDataOutputMessage
    {
        int Nose;
        int Core;
        int Engine;
        public int[] Weapons;
        byte[] FireGroups;
        public ShipData GeneratedData;

        public ShipDataOutputMessage(NetIncomingMessage msg)
        {
            Nose = msg.ReadInt32();
            Core = msg.ReadInt32();
            Engine = msg.ReadInt32();

            GeneratedData = new ShipData(Nose, Core, Engine);

            GeneratedData.NoseColor = new Color(msg.ReadByte(), msg.ReadByte(), msg.ReadByte());
            GeneratedData.CoreColor = new Color(msg.ReadByte(), msg.ReadByte(), msg.ReadByte());
            GeneratedData.TailColor = new Color(msg.ReadByte(), msg.ReadByte(), msg.ReadByte());
            GeneratedData.WeaponColor = new Color(msg.ReadByte(), msg.ReadByte(), msg.ReadByte());

            int length = msg.ReadInt32();
            Weapons = new int[length];
            FireGroups = new byte[length];
            for (int i = 0; i < length; i++)
            {
                Weapons[i] = msg.ReadInt32();
                FireGroups[i] = msg.ReadByte();
                Vector3? freeSlot = FreeSlot(GeneratedData.Nose);
                if (freeSlot != null)
                {
                    GeneratedData.SetWeapon((Vector3)freeSlot, Weapons[i], FireGroups[i]);
                    continue;
                }
                freeSlot = FreeSlot(GeneratedData.Core);
                if (freeSlot != null)
                {
                    GeneratedData.SetWeapon((Vector3)freeSlot, Weapons[i], FireGroups[i]);
                    continue;
                }
                freeSlot = FreeSlot(GeneratedData.Engine);
                if (freeSlot != null)
                {
                    GeneratedData.SetWeapon((Vector3)freeSlot, Weapons[i], FireGroups[i]);
                    continue;
                }
            }
            GeneratedData.WeaponIDs = Weapons;
        }

        Vector3? FreeSlot(PartData part)
        {
            if (part.WeaponSlots == null) return null;
            foreach (Vector3 offset in part.WeaponSlots)
            {
                if (GeneratedData.Weapons[offset] == null) return offset;
            }
            return null;
        }
    }
}
