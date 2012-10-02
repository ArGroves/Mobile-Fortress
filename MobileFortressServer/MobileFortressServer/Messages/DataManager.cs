using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using MobileFortressServer.Messages;
using Microsoft.Xna.Framework;
using MobileFortressServer;
using MobileFortressServer.Ships;
using MobileFortressServer.Data;
using MobileFortressServer.Physics;
using Microsoft.Xna.Framework.Input;

namespace MobileFortressClient.Messages
{
    class DataManager
    {
        public void HandleData(NetIncomingMessage msg)
        {
            NetMsgType datatype = (NetMsgType)msg.ReadByte();
            if (datatype == NetMsgType.Control)
            {
                Soul soul = (Soul)msg.SenderConnection.Tag;
                var key = (ControlKey)msg.ReadByte();
                var edge = msg.ReadBoolean();
                //Console.WriteLine("Received Control Message: " + key + (edge ? "Pressed" : "Released"));
                soul.currentShip.Controls.ReceiveMessage(key, edge);
            }
            else if (datatype == NetMsgType.ControlUpdate)
            {
                Soul soul = (Soul)msg.SenderConnection.Tag;
                var Pitch = msg.ReadFloat();
                var Yaw = msg.ReadFloat();
                var Roll = msg.ReadFloat();
                soul.currentShip.Controls.ReceiveMessage(Pitch, Yaw, Roll);
            }
            else if (datatype == NetMsgType.ShipDataOutput)
            {
                ShipDataOutputMessage data;
                data = new ShipDataOutputMessage(msg);
                Soul soul = new Soul(msg.SenderConnection);
                soul.currentSector = Sector.Redria;
                msg.SenderConnection.Tag = soul;

                soul.currentSector.Terrain.Load(msg.SenderConnection);
                soul.currentSector.Ships.Load(msg.SenderConnection);
                soul.currentSector.Objects.Load(msg.SenderConnection);
                var idRandomizer = new Random();

                var NewShip = new ShipObj(new Vector3(0, 100, 2), Quaternion.Identity,
                    data.GeneratedData);
                soul.currentShip = NewShip;
                NewShip.Client = soul;
                //Console.WriteLine(msg.SenderEndpoint.Address + "'s Ship ID: " + NewShip.ID);
                MessageWriter.ClientEntityCreationMessage(soul.Owner,
                    NewShip.ID, NewShip.Position, NewShip.Orientation,
                    data.GeneratedData.NoseID, data.GeneratedData.CoreID, data.GeneratedData.EngineID,
                    data.Weapons, msg.SenderConnection.RemoteUniqueIdentifier);
            }
        }
    }
}
