using System;

namespace MapViewer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (MapViewer game = new MapViewer())
            {
                game.Run();
            }
        }
    }
}

