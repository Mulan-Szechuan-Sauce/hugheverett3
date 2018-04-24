﻿using Ninject;
using System;
using System.Reflection;

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
        public static void Main()
        {
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());

            using (var game = kernel.Get<HughGame>())
                game.Run();
        }
    }
}
