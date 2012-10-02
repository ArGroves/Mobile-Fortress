using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Lidgren.Network;
using MobileFortressServer.Messages;
using MobileFortressServer.Managers;

namespace MobileFortressServer.Physics
{
    class WorldObj
    {
        public virtual Vector3 Position { get; set; }
        public virtual Quaternion Orientation { get; set; }
        public virtual Vector3 Velocity { get; set; }
        public UInt16 resource_index = 0; //Index for the client-side resources this object has.
        public ushort ID { get; set; }

        public virtual void Encode(NetOutgoingMessage msg)
        {
            msg.Write(ID);
            msg.Write(Position.X); msg.Write(Position.Y); msg.Write(Position.Z);
            msg.Write(Velocity.X); msg.Write(Velocity.Y); msg.Write(Velocity.Z);
            msg.Write(Orientation.X); msg.Write(Orientation.Y);
            msg.Write(Orientation.Z); msg.Write(Orientation.W);
        }
    }
}
