using System;

namespace Hugh
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string levelName = "level3";
            using (var game = new Game1(levelName))
                game.Run();
        }
    }
}
