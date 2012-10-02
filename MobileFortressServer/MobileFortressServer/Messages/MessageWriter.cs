using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Lidgren.Network;
using MobileFortressServer.Messages;
using MobileFortressServer;

namespace MobileFortressServer.Messages
{
    class MessageWriter
    {
        public static void ClientEntityCreationMessage(NetConnection c, NetEntityType type, ushort id, Vector3 position, Quaternion orientation, ushort resource)
        {
            var msg = Network.Server.CreateMessage();
            msg.Write((byte)NetMsgType.CreateOnClient);
            msg.Write((byte)type);
            msg.Write(id);
            msg.Write(position.X); msg.Write(position.Y); msg.Write(position.Z);
            msg.Write(orientation.X); msg.Write(orientation.Y);
            msg.Write(orientation.Z); msg.Write(orientation.W);
            msg.Write(resource);
            if (c != null)
                Network.Server.SendMessage(msg, c, NetDeliveryMethod.ReliableOrdered);
            else
                Network.Server.SendToAll(msg, NetDeliveryMethod.ReliableOrdered);
        }
        public static void ClientEntityCreationMessage(NetConnection c, ushort id, Vector3 position, Quaternion orientation, int nose, int core, int tail, int[] weapons, long ownerID)
        {
            var msg = Network.Server.CreateMessage();
            msg.Write((byte)NetMsgType.CreateOnClient);
            msg.Write((byte)NetEntityType.Ship);
            msg.Write(id);
            msg.Write(position.X); msg.Write(position.Y); msg.Write(position.Z);
            msg.Write(orientation.X); msg.Write(orientation.Y);
            msg.Write(orientation.Z); msg.Write(orientation.W);
                msg.Write(nose);
                msg.Write(core);
                msg.Write(tail);
                if (weapons != null && weapons.Length > 0)
                {
                    msg.Write(weapons.Length);
                    for (int i = 0; i < weapons.Length; i++)
                        msg.Write(weapons[i]);
                }
                else
                {
                    msg.Write((int)0);
                }
            msg.Write(ownerID);
            if (c != null)
                Network.Server.SendMessage(msg, c, NetDeliveryMethod.ReliableOrdered);
            else
                Network.Server.SendToAll(msg, NetDeliveryMethod.ReliableOrdered);
        }
        public static void ExplosionMessage(ushort id, float radius)
        {
            var msg = Network.Server.CreateMessage();
            msg.Write((byte)NetMsgType.Explosion);
            msg.Write(id);
            msg.Write(radius);
            Network.Server.SendToAll(msg, NetDeliveryMethod.ReliableUnordered);
        }
        public static void FireBulletMessage(ushort type, Vector3 position, Quaternion orientation, Vector3 velocity)
        {
            var msg = Network.Server.CreateMessage();
            msg.Write((byte)NetMsgType.Bullet);
            msg.Write(type);
            msg.Write(position.X); msg.Write(position.Y); msg.Write(position.Z);
            msg.Write(velocity.X); msg.Write(velocity.Y); msg.Write(velocity.Z);
            msg.Write(orientation.X); msg.Write(orientation.Y);
            msg.Write(orientation.Z); msg.Write(orientation.W);
            Network.Server.SendToAll(msg, NetDeliveryMethod.ReliableUnordered);
        }

        public static void MapLoadingMessage(NetConnection c, int seed, int power, float roughness)
        {
            var msg = Network.Server.CreateMessage();
            msg.Write((byte)NetMsgType.MapLoading);
            msg.Write((byte)0);
            msg.Write(seed);
            msg.Write(power);
            msg.Write(roughness);
            Network.Server.SendMessage(msg, c, NetDeliveryMethod.ReliableOrdered);
        }
        public static void MapLoadingMessage(NetConnection c, Rectangle[] areas)
        {
            var msg = Network.Server.CreateMessage();
            msg.Write((byte)NetMsgType.MapLoading);
            msg.Write((byte)1);
            msg.Write((int)areas.Length);
            Console.WriteLine(areas.Length + " Map Alterations.");
            foreach(Rectangle r in areas)
            {
                //Console.WriteLine("{" + r.X + ", " + r.Y + " : " + r.Width + "x" + r.Height + "}");
                msg.Write((int)r.X);
                msg.Write((int)r.Y);
                msg.Write((int)r.Width);
                msg.Write((int)r.Height);
            }
            Network.Server.SendMessage(msg, c, NetDeliveryMethod.ReliableOrdered);
        }
        public static void ShipExplosionMessage(ushort shipID)
        {
            var msg = Network.Server.CreateMessage();
            msg.Write((byte)NetMsgType.ShipExplode);
            msg.Write(shipID);
            Network.Server.SendToAll(msg, NetDeliveryMethod.ReliableUnordered);
        }
        public static void ChatMessage(NetConnection c, String data)
        {
            var msg = Network.Server.CreateMessage();
            msg.Write((byte)NetMsgType.Chat);
            msg.Write(data);
            Network.Server.SendMessage(msg, c, NetDeliveryMethod.ReliableUnordered);
        }
        public static void MissileLockonMessage(NetConnection c, ushort shipID, LockonStatus status)
        {
            var msg = Network.Server.CreateMessage();
            msg.Write((byte)NetMsgType.Lockon);
            msg.Write(shipID);
            msg.Write((byte)status);
            Network.Server.SendMessage(msg, c, NetDeliveryMethod.ReliableUnordered);
        }
        public static void EntityUpdateMessage(NetConnection c, ushort id, NetEntityType type, Vector3 position, Quaternion orientation, Vector3 velocity)
        {
            var msg = Network.Server.CreateMessage();
            msg.Write((byte)NetMsgType.EntityUpdate);
            msg.Write((byte)type);
            msg.Write(id);
            msg.Write(position.X); msg.Write(position.Y); msg.Write(position.Z);
            msg.Write(velocity.X); msg.Write(velocity.Y); msg.Write(velocity.Z);
            msg.Write(orientation.X); msg.Write(orientation.Y);
            msg.Write(orientation.Z); msg.Write(orientation.W);
            Network.Server.SendMessage(msg, c, NetDeliveryMethod.UnreliableSequenced, 1);
        }
        public static void StatusMessage(NetConnection c, float health, ushort[] ammo)
        {
            var msg = Network.Server.CreateMessage();
            msg.Write((byte)NetMsgType.ShipUpdate);
            msg.Write(health);
            msg.Write((byte)ammo.Length);
            for (byte i = 0; i < ammo.Length; i++)
            {
                msg.Write(ammo[i]);
            }
            Network.Server.SendMessage(msg, c, NetDeliveryMethod.UnreliableSequenced, 0);
        }
    }
}
