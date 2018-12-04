using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace reversi
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void board_SquareClicked(Point square)
        {
            board.MakeMove(square.X, square.Y);
        }

        private void board_UpdateStatus()
        {
            // Get the scores
            int redScore = board.board.Score(Piece.Red);
            int blueScore = board.board.Score(Piece.Blue);

            // Update the scores
            labelScoreBlue.Text = blueScore.ToString();
            labelScoreRed.Text = redScore.ToString();

            // Update the status label
            if(board.GameEnded)
            {
                if(redScore > blueScore)
                    labelStatus.Text = "Red has won";
                else if(redScore == blueScore)
                    labelStatus.Text = "It is a draw";
                else if(redScore < blueScore)
                    labelStatus.Text = "Blue has won";
            }
            else
            {
                if(board.LastPassed)
                    labelStatus.Text = board.CurrentTurn == Piece.Red ? "Blue had to pass" : "Red had to pass";
                else
                    labelStatus.Text = board.CurrentTurn == Piece.Red ? "Red is on move" : "Blue is on move";
            }
        }

        private void buttonNewGame_Click(object sender, EventArgs e)
        {
            board.ClearBoard();
        }

        private void checkHelp_CheckedChanged(object sender, EventArgs e)
        {
            board.ShowHints = checkHelp.Checked;
            board.RefreshBoard();
        }
    }
}
