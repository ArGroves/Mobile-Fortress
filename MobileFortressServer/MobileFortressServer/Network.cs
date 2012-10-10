using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using MobileFortressServer.Physics;
using MobileFortressServer.Messages;
using Microsoft.Xna.Framework;
using MobileFortressServer.Ships;
using MobileFortressClient.Messages;

namespace MobileFortressServer
{
    class Network
    {
        public static NetServer Server;

        const ushort Version = 1;

        const int Port = 1357;

        static double lastStatus = NetTime.Now;
        const double ServerTick = 1d/30d;

        static int ships = 0;

        static DataManager Manager = new DataManager();

        public static void AddShip()
        {
            ships++;
        }

        public static void Initialize()
        {
            NetPeerConfiguration config = new NetPeerConfiguration("Mobile Fortress");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.EnableMessageType(NetIncomingMessageType.UnconnectedData);
            config.Port = Port;
            config.SimulatedMinimumLatency = 0.0025f;
            config.SimulatedRandomLatency = 0.002f;
            config.SimulatedDuplicatesChance = 0.00005f;
            config.SimulatedLoss = 0.0001f;
            //config.EnableUPnP = true;

            Server = new NetServer(config);
            Server.Start();
        }
        public static void Shutdown()
        {
            Server.Shutdown("Program ended.");
            Console.WriteLine("Server has shut down.");
        }

        public static void Process(double time)
        {
            Incoming();
            if (time > lastStatus)
            {
                Outgoing();
                lastStatus += ServerTick;
            }
        }
        enum ConnectMsgType { NewUserCreated, WrongPassword, LoginSuccess }
        static void Incoming()
        {
            NetIncomingMessage msg;
            while ((msg = Server.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.DiscoveryRequest:
                        Console.WriteLine("Request from "+msg.SenderEndpoint.Address + " - Port " + msg.SenderEndpoint.Port);
                        NetOutgoingMessage reply = Server.CreateMessage();
                        reply.Write(Version);
                        Server.SendDiscoveryResponse(reply, msg.SenderEndpoint);
                        Console.WriteLine("Sending response to:" + msg.SenderEndpoint.Address + " - Port " + msg.SenderEndpoint.Port);
                        break;

                    case NetIncomingMessageType.StatusChanged:
                        NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();
                        Console.WriteLine(msg.SenderEndpoint.Address + " " + status);
                        break;

                    case NetIncomingMessageType.Data:
                        Manager.HandleData(msg);
                        break;

                    case NetIncomingMessageType.UnconnectedData:
                        var datatype = (NetMsgType)msg.ReadByte();
                        if (datatype == NetMsgType.Login)
                        {
                            var udata = new ConnectionMessage(msg);
                            if (!UserData.UserExists(udata.Username))
                            {
                                UserData.Add(udata.Username, udata.Password);
                                var outmsg = Server.CreateMessage();
                                outmsg.Write((byte)ConnectMsgType.NewUserCreated);
                                Server.SendUnconnectedMessage(outmsg, msg.SenderEndpoint);
                            }
                            else
                            {
                                if (UserData.Check(udata.Username, udata.Password))
                                {
                                    var outmsg = Server.CreateMessage();
                                    outmsg.Write((byte)ConnectMsgType.LoginSuccess);
                                    Server.SendUnconnectedMessage(outmsg, msg.SenderEndpoint);
                                }
                                else
                                {
                                    var outmsg = Server.CreateMessage();
                                    outmsg.Write((byte)ConnectMsgType.WrongPassword);
                                    Server.SendUnconnectedMessage(outmsg, msg.SenderEndpoint);
                                }
                            }
                        }
                        break;
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.VerboseDebugMessage:
                    case NetIncomingMessageType.Error:
                    case NetIncomingMessageType.ErrorMessage:
                    case NetIncomingMessageType.WarningMessage:
                        Console.WriteLine(msg.ReadString());
                        break;

                    default:
                        Console.WriteLine("Unhandled Message Type: " + msg.MessageType);
                        break;
                }
                Server.Recycle(msg);
            }
        }
        static void Outgoing()
        {
            foreach (NetConnection connection in Server.Connections)
            {
                if (connection.Tag == null) continue;
                Soul soul = (Soul)connection.Tag;
                if (soul.currentShip != null)
                {
                    soul.currentShip.SendStatus(connection);
                    soul.currentSector.SendUpdate(connection);
                }
            }
        }
    }
}
