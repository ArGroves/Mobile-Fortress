using System;
using MobileFortressServer.Physics;
using Lidgren.Network;
using System.Runtime.InteropServices;

namespace MobileFortressServer
{
#if WINDOWS || XBOX
    static class Program
    {
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(CtrlType sig);
        static EventHandler _handler;

        enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private static bool Handler(CtrlType sig)
        {
            switch (sig)
            {
                case CtrlType.CTRL_C_EVENT:
                case CtrlType.CTRL_LOGOFF_EVENT:
                case CtrlType.CTRL_SHUTDOWN_EVENT:
                case CtrlType.CTRL_CLOSE_EVENT:
                default:
                    Network.Shutdown();
                    return false;
            }
        }

        const double programSpeed = 1d / 30d;
        static double lastUpdate = NetTime.Now;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            double time = NetTime.Now;
            _handler += Handler;
            SetConsoleCtrlHandler(_handler, true);
            Initialize();
            while (true)
            {
                time = NetTime.Now;
                Update(time);
            }
        }

        static void Initialize()
        {
            Console.WriteLine("Mobile Fortress Server Alpha Build 1");
            Network.Initialize();
            Sector.Redria.Initialize();
        }

        static void Update(double time)
        {
            if (time > lastUpdate)
            {
                //Console.WriteLine("Processing Network with lastUpdate: " + (lastUpdate) + " time: " + time);
                Network.Process(time);
                //Console.WriteLine("Updating Redria with lastUpdate: "+(lastUpdate)+" dt: "+programSpeed);
                Sector.Redria.Update((float)programSpeed);
                lastUpdate += programSpeed;
                //Console.WriteLine("Next Update: " + (lastUpdate));
            }
        }
    }
#endif
}

