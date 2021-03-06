﻿using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace reversi
{
    public partial class MainWindow : Form, IGameObserver
    {
        public MainWindow()
        {
            InitializeComponent();
            PlayerContorller = humanPlayer;
        }

        public IPlayerController PlayerContorller { get; set; }

        public Game Game { get; set; }

        public Task OnMove(Board b, MoveDescriptor md)
        {
            Action d = () => board_UpdateStatus();
            Invoke(d);
            return Task.FromResult(true);
        }

        private void board_UpdateStatus()
        {
            Action refresh = () =>
            {
                // Get the scores
                int redScore = Game.Board.Score(Piece.Red);
                int blueScore = Game.Board.Score(Piece.Blue);

                // Update the scores
                labelScoreBlue.Text = blueScore.ToString();
                labelScoreRed.Text = redScore.ToString();

                // Update the status label
                if (humanPlayer.GameEnded)
                {
                    if (redScore > blueScore)
                    {
                        labelStatus.Text = "Red has won";
                    }
                    else if (redScore == blueScore)
                    {
                        labelStatus.Text = "It is a draw";
                    }
                    else if (redScore < blueScore)
                    {
                        labelStatus.Text = "Blue has won";
                    }
                }
                else
                {
                    if (humanPlayer.LastPassed)
                    {
                        labelStatus.Text = humanPlayer.CurrentTurn == Piece.Red ? "Blue had to pass" : "Red had to pass";
                    }
                    else
                    {
                        labelStatus.Text = humanPlayer.CurrentTurn == Piece.Red ? "Red is on move" : "Blue is on move";
                    }
                }
            };
            this.Invoke(refresh);
        }

        private async void buttonNewGame_Click(object sender, EventArgs e)
        {
            await Game.ResetGame();
            await Game.PlayAsync();
        }

        private void checkHelp_CheckedChanged(object sender, EventArgs e)
        {
            humanPlayer.ShowHints = checkHelp.Checked;
            humanPlayer.RefreshBoard();
        }
    }
}
