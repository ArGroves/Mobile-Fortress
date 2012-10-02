using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Lidgren.Network;

namespace MobileFortressServer.Messages
{
    struct EntityUpdateMessage
    {
        public EntityUpdateMessage(NetOutgoingMessage msg, ushort id, NetEntityType type, Vector3 position, Quaternion orientation, Vector3 velocity)
        {
            msg.Write((byte)NetMsgType.EntityUpdate);
            msg.Write((byte)type);
            msg.Write(id);
            msg.Write(position.X); msg.Write(position.Y); msg.Write(position.Z);
            msg.Write(velocity.X); msg.Write(velocity.Y); msg.Write(velocity.Z);
            msg.Write(orientation.X); msg.Write(orientation.Y);
            msg.Write(orientation.Z); msg.Write(orientation.W);
        }
    }
}
