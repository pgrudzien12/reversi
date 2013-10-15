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
            InitializeComponent(this);
        }

        private void board_SquareClicked(Point square)
        {
            if (board.MakeMove(square.X, square.Y, board.currStatus.currTurn))
                board.currStatus.currTurn = board.currStatus.currTurn == Board.Piece.Red ? Board.Piece.Blue : Board.Piece.Red;
        }

        public void showScores(Board.GameStatus status)
        {
            scoreLabelBlue.Text = status.score[(int)Board.Piece.Blue].ToString();
            scoreLabelRed.Text = status.score[(int)Board.Piece.Red].ToString();
        }

        public void showStatus(string status)
        {
            statusLabel.Text = status;
        }
    }
}
