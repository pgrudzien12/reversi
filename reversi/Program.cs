using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace reversi
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetCompatibleTextRenderingDefault(false);
            var window = new MainWindow();
            Game reversiGame = new Game(window.PlayerContorller, new RandomMovesPlayer());
            reversiGame.AddObserver(window);
            window.Game = reversiGame;
            Task t = reversiGame.PlayAsync();
            Application.EnableVisualStyles();
            Application.Run(window);
        }
    }
}
