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
        /// <summary>Whose turn it currently is</summary>
        private Board.Piece currTurn = Board.Piece.Red;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void board_SquareClicked(Point square)
        {
            if(board.MakeMove(square.X, square.Y, currTurn))
                currTurn = currTurn == Board.Piece.Red ? Board.Piece.Blue : Board.Piece.Red;
        }
    }
}
