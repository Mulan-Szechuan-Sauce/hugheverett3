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
            string levelName = "level2";
            using (var game = new HughGame(levelName))
                game.Run();
        }
    }
}
