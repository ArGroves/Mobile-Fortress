using System;

namespace MobileFortressClient
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (MobileFortressClient game = new MobileFortressClient())
            {
                game.Run();
            }
        }
    }
#endif
}

