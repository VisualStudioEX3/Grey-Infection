using System;

namespace WhiteAndWorld
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            try
            {
                using (GameBase game = new GameBase())
                {
                    game.Run();
                }
            }
            catch (Exception e)
            {
                using (CrashDebugGame game = new CrashDebugGame(e, @"fonts\default"))
                {
                    game.Run();
                }
            }
        }
    }
#endif
}

