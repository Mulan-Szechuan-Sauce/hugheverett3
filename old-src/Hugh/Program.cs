using System;
using System.Text.RegularExpressions;

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
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                using (var game = new HughGame())
                    game.Run();
            }
            else if (args.Length == 1)
            {
                // Get path relative to the Content directory
                Regex rgx = new Regex("^.*\\/Content\\/");
                string levelPath = rgx.Replace(args[0], "");
                // Without the extension
                string levelName = levelPath.Substring(0, levelPath.Length - 4);

                using (var game = new HughGame(levelName))
                    game.Run();
            }
            else
            {
                Console.WriteLine("Invalid arguments.\nUsage: hugh [levelName]");
            }
        }
    }
}
