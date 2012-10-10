using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lidgren.Network;
using MobileFortressClient.Physics;
using Microsoft.Xna.Framework;
using MobileFortressClient.Messages;
using MobileFortressClient.Ships;
using MobileFortressClient.Data;
using Microsoft.Xna.Framework.Audio;
using MobileFortressClient.Managers;
using MobileFortressClient.Menus;
using MobileFortressClient.ClientObjects;
using MobileFortressClient.MobileObjects;

namespace MobileFortressClient
{
    class Network
    {
        public static NetClient Client;

        const UInt16 Version = 1;

        const int Port = 1358;
        const int ServerPort = 1357;

        static double lastUpdate = NetTime.Now;
        const double ServerTick = (1d / 30d);
        public static float Interpolation = 0.3f;

        public static bool IsConnected = false;
        public static bool JoinedGame = false;

        public static string Username { private get; set; }
        public static string Password { private get; set; }
        public static string NetworkAddress { get; set; }

        static MobileFortressClient Game;

        static DataManager Manager = new ShipDataManager();

        public static void Initialize(MobileFortressClient game)
        {
            Game = game;
            NetPeerConfiguration config = new NetPeerConfiguration("Mobile Fortress");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            config.EnableMessageType(NetIncomingMessageType.UnconnectedData);
            config.EnableMessageType(NetIncomingMessageType.WarningMessage);
            config.EnableMessageType(NetIncomingMessageType.DebugMessage);
            //config.EnableUPnP = true;
            config.Port = Port;
            config.SimulatedMinimumLatency = 0.0025f;
            config.SimulatedRandomLatency = 0.002f;
            config.SimulatedLoss = 0.0001f;

            Client = new NetClient(config);
            Client.Start();
            NetworkAddress = "127.0.0.1";
        }

        public static void Shutdown()
        {
            Client.Shutdown("Program ended.");
        }
        
        /*HAAAX*/
        public static void FindServer()
        {
            Console.WriteLine("Requesting server at " + NetworkAddress);
            Client.DiscoverKnownPeer(NetworkAddress, ServerPort);
        }

        public static void FindServer(string address)
        {
            Console.WriteLine("Requesting server at "+address);
            Client.DiscoverKnownPeer(address, ServerPort);
        }

        public static void Process(double time)
        {
            Incoming();
            if (time > lastUpdate && IsConnected && JoinedGame)
            {
                Outgoing();
                lastUpdate += ServerTick;
            }
        }
        static void Incoming()
        {
            NetIncomingMessage msg;
            while ((msg = Client.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.DiscoveryResponse:
                        Console.WriteLine("Response from " + msg.SenderEndpoint.Address);
                        var loginmsg = Client.CreateMessage();
                        new ConnectionMessage(loginmsg, Username, Password);
                        Client.SendUnconnectedMessage(loginmsg, msg.SenderEndpoint);
                        break;

                    case NetIncomingMessageType.StatusChanged:
                        NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();
                        Console.WriteLine(status + " - " + msg.SenderEndpoint.Address );
                        if (status == NetConnectionStatus.Connected)
                        {
                            IsConnected = true;
                            MenuManager.Menu = new ShipCustomizer();
                        }
                        break;

                    case NetIncomingMessageType.UnconnectedData:
                        if (IsConnected) break;//Do not accept unconnected data when connected.
                        var subdata = (ConnectMsgType)msg.ReadByte();
                        TitleScreen menu = (TitleScreen)MenuManager.Menu;
                        switch (subdata)
                        {
                            case ConnectMsgType.WrongPassword:
                                menu.WrongPassword();
                                break;
                            case ConnectMsgType.NewUserCreated:
                            case ConnectMsgType.LoginSuccess:
                                menu.Login();
                                Client.Connect(msg.SenderEndpoint);
                                break;
                        }
                        break;

                    case NetIncomingMessageType.Data:
                        Manager.HandleData(msg);
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
                Client.Recycle(msg);
            }
        }
        static void Outgoing()
        {
            MessageWriter.ControlUpdateMessage(Controls.Instance.Pitch, Controls.Instance.Yaw, Controls.Instance.Roll);
        }

        public static void SendControlMsg(bool edge, ControlKey key)
        {
            Console.WriteLine("Sending Control Message: " + key + (edge ? "Pressed" : "Released"));
            MessageWriter.ControlMessage(key, edge);
        }
    }
}

