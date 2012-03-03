using System;

namespace Planets
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Planets game = new Planets())
            {
                game.Run();
            }
        }
    }
#endif
}

