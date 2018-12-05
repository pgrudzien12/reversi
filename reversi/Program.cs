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
            Game reversiGame = new Game(window.PlayerContorller, new MinMaxPlayer(6));
            reversiGame.AddObserver(window);
            reversiGame.AddObserver(window.PlayerContorller);
            window.Game = reversiGame;
            Task t = Task.Run(reversiGame.PlayAsync);
            Application.EnableVisualStyles();
            Application.Run(window);
        }
    }
}
