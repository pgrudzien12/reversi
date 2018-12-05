using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace reversi
{
    public partial class BoardControl : Control, IPlayerController
    {
        private int WIDTH = Board.WIDTH;
        private int HEIGHT = Board.HEIGHT;

        public Board board  = new Board();
        private TaskCompletionSource<MoveDescriptor> tcs1;

        internal void MakeMove(int x, int y)
        {
            if (board.MakeMove(x, y))
            {
                RefreshBoard();
            }
        }

        /// <summary>Which square currently should be highlighted, or (-1, -1) if none should be highlighted</summary>
        private Point highlightSquare = new Point(-1, -1);

        /// <summary>The square where the left mouse button was pressed, or (-1, -1) if the left mouse button currently isn't down</summary>
        private Point mouseDownSquare = new Point(-1, -1);
        private bool _showHints;

        /// <summary>The form that a SquareClicked event handler should have</summary>
        /// <param name="square">The coordinates (in columns and rows) of the square that was clicked</param>
        public delegate void SquareClickedEventHandler(Point square);

        /// <summary>The event that is fired when a certain square is clicked</summary>
        public event SquareClickedEventHandler SquareClicked;

        /// <summary>The form that a UpdateStatus event handler should have</summary>
        public delegate void UpdateStatusEventHandler();

        /// <summary>The event that is fired when a certain square is clicked</summary>
        public event UpdateStatusEventHandler UpdateStatus;

        /// <summary>Default constructor</summary>
        public BoardControl()
        {
            // Initialize the control
            InitializeComponent();

            // Redraw when the control is resized
            this.ResizeRedraw = true;

            // Initialize the board
            ClearBoard();

            // Initially show the hints
            _showHints = true;
        }

        /// <summary>Clears the board, places the initial pieces and resets the game status</summary>
        public void ClearBoard()
        {
            board.ClearBoard();

            // Refresh the board
            RefreshBoard();
        }

        /// <summary>Refreshes the board, i.e. the board is redrawn and an UpdateStatus event is triggered</summary>
        public void RefreshBoard()
        {
            // Repaint
            Invalidate();

            // Trigger the UpdateStatus event
            UpdateStatus?.Invoke();
        }

        /// <summary>Whose turn it currently is</summary>
        public Piece CurrentTurn
        { get { return board.currStatus.currTurn; } }

        /// <summary>Whether or not the game has ended</summary>
        public bool GameEnded
        { get { return board.currStatus.gameEnded; } }

        /// <summary>Whether or not to show hints</summary>
        public bool ShowHints
        {
            get { return _showHints; }
            set
            {
                _showHints = value;
                RefreshBoard();
            }
        }

        /// <summary>Whether or not the last player has passed</summary>
        public bool LastPassed
        { get { return board.currStatus.lastPassed; } }




        /// <summary>Draws a smileys that looks towards the mouse</summary>
        /// <param name="g">The graphics object that's used to draw the smiley</param>
        /// <param name="color">The color to use for the background of the smiley</param>
        /// <param name="x">The x-coordinate of the top-left corner of the smiley</param>
        /// <param name="y">The y-coordinate of the top-left corner of the smiley</param>
        /// <param name="size">The size (diameter) of the smiley</param>
        private void drawSmiley(Graphics g, Brush color, int x, int y, int size)
        {
            // If the size is zero (or smaller), there is nothing to do
            if (size <= 0.0f)
            {
                return;
            }

            // Draw the background of the smiley
            g.FillEllipse(color, x, y, size, size);

            // Draw the mouth
            //GraphicsPath mouthPath = new GraphicsPath();
            //mouthPath.StartFigure();
            //mouthPath.AddArc(x + 5.0f * size / 32, y + 3.0f * size / 32.0f, 11.0f * size / 16, 13.0f * size / 16, 0.0f, 180.0f);
            //mouthPath.CloseFigure();
            //g.FillPath(Brushes.White, mouthPath);
            //g.DrawPath(Pens.Black, mouthPath);

            //// Draw the eyes
            //drawSmileyEye(g, x + 5.0f * size / 32, y + size / 8.0f, 5.0f * size / 16);
            //drawSmileyEye(g, x + 17.0f * size / 32, y + size / 8.0f, 5.0f * size / 16);
        }

        /// <summary>Helper function for drawSmiley(), this draws an eye at the given location</summary>
        /// <param name="g">The graphics object that's used to draw the eye</param>
        /// <param name="x">The x-coordinate of the top-left corner of the eye</param>
        /// <param name="y">The y-coordinate of the top-left corner of the eye</param>
        /// <param name="size">The size (diameter) of the eye</param>
        private void drawSmileyEye(Graphics g, float x, float y, float size)
        {
            // Get the mouse coordinates in client coordinates and add the translation of the graphics object
            Point mouseCoords = PointToClient(MousePosition);
            mouseCoords.X -= (int)g.Transform.OffsetX;
            mouseCoords.Y -= (int)g.Transform.OffsetY;

            // Determine the center x and center y of the smiley
            float centerX = x + size / 2;
            float centerY = y + size / 2;

            // Draw the background of the eye
            g.FillEllipse(Brushes.White, x, y, size, size);
            g.DrawEllipse(Pens.Black, x, y, size, size);

            // Draw the pupil
            float pupilRadius = size / 10;
            float dst = (float)Math.Sqrt(Math.Pow(mouseCoords.X - centerX, 2) + Math.Pow(mouseCoords.Y - centerY, 2));
            if (dst <= size / 2 - pupilRadius)
            {
                g.FillEllipse(Brushes.Black, mouseCoords.X - pupilRadius, mouseCoords.Y - pupilRadius, pupilRadius * 2, pupilRadius * 2);
            }
            else
            {
                float factor = (size / 2 - pupilRadius) / dst;
                g.FillEllipse(Brushes.Black, centerX - pupilRadius + (mouseCoords.X - centerX) * factor, centerY - pupilRadius + (mouseCoords.Y - centerY) * factor, pupilRadius * 2, pupilRadius * 2);
            }
        }

        private void Board_Paint(object sender, PaintEventArgs pea)
        {
            // We want fancy graphics :D
            pea.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Calculate the size of a square, and the translation of the board (so it's centred)
            int squareSize = Math.Min(Width / WIDTH, Height / HEIGHT);
            pea.Graphics.TranslateTransform((Width - WIDTH * squareSize) / 2, (Height - HEIGHT * squareSize) / 2);

            // If the control is too small, we stop here
            if (squareSize == 0)
            {
                return;
            }

            // Draw the highlighted square, but only if the game hasn't ended yet
            if (board.currStatus.gameEnded)
            {
                Cursor = Cursors.Default;
            }
            else if (mouseDownSquare.X >= 0 && mouseDownSquare.Y >= 0 && mouseDownSquare.X < WIDTH && mouseDownSquare.Y < HEIGHT)
            {
                if (board[mouseDownSquare.X, mouseDownSquare.Y] == Piece.None)
                {
                    pea.Graphics.FillRectangle(Brushes.Black, mouseDownSquare.X * squareSize, mouseDownSquare.Y * squareSize, squareSize, squareSize);
                    Cursor = Cursors.Hand;
                }
                else
                {
                    Cursor = Cursors.Default;
                }
            }
            else if (highlightSquare.X >= 0 && highlightSquare.Y >= 0 && highlightSquare.X < WIDTH && highlightSquare.Y < HEIGHT && board[highlightSquare.X, highlightSquare.Y] == Piece.None)
            {
                pea.Graphics.FillRectangle(Brushes.DarkGray, highlightSquare.X * squareSize, highlightSquare.Y * squareSize, squareSize, squareSize);
                Cursor = Cursors.Hand;
            }
            else
            {
                Cursor = Cursors.Default;
            }

            // Draw the board
            for (int i = 0; i < WIDTH; ++i)
            {
                pea.Graphics.DrawLine(Pens.Black, i * squareSize, 0, i * squareSize, HEIGHT * squareSize - 1);
            }

            pea.Graphics.DrawLine(Pens.Black, WIDTH * squareSize - 1, 0, WIDTH * squareSize - 1, HEIGHT * squareSize - 1);
            for (int i = 0; i < HEIGHT; ++i)
            {
                pea.Graphics.DrawLine(Pens.Black, 0, i * squareSize, WIDTH * squareSize - 1, i * squareSize);
            }

            pea.Graphics.DrawLine(Pens.Black, 0, HEIGHT * squareSize - 1, WIDTH * squareSize - 1, HEIGHT * squareSize - 1);

            // Draw the pieces
            for (int x = 0; x < WIDTH; ++x)
            {
                for (int y = 0; y < HEIGHT; ++y)
                {
                    if (board[x, y] == Piece.None)
                    {
                        continue;
                    }

                    drawSmiley(pea.Graphics, GetBrushColor(x, y), x * squareSize + 1, y * squareSize + 1, squareSize - 2);
                }
            }

            // Draw the hints (all possible moves)
            if (_showHints)
            {
                MoveDescriptor[] validMoves = board.ValidMoves(board.currStatus.currTurn);
                foreach (MoveDescriptor validMove in validMoves)
                {
                    drawSmiley(pea.Graphics, Brushes.Gray, validMove.X * squareSize + 20, validMove.Y * squareSize + 20, squareSize / 4);
                }
            }
        }

        private Brush GetBrushColor(int x, int y)
        {
            return board[x, y] == Piece.Blue ? Brushes.Blue : Brushes.Red;
        }

        private void Board_MouseMove(object sender, MouseEventArgs mea)
        {
            int squareSize = Math.Min(Width / WIDTH, Height / HEIGHT);    // Calculate the size of a square
            int transX = (Width - WIDTH * squareSize) / 2;                      // The horizontal translation of the board
            int transY = (Height - HEIGHT * squareSize) / 2;                    // The vertical translation of the board

            // If the control is too small, we stop here
            if (squareSize == 0)
            {
                return;
            }

            // Calculate the square that we're hovering above and redraw the board
            highlightSquare = new Point((mea.X - transX) / squareSize, (mea.Y - transY) / squareSize);
            if (mea.X < transX || mea.Y < transY || highlightSquare.X < 0 || highlightSquare.Y < 0 || highlightSquare.X >= WIDTH || highlightSquare.Y >= HEIGHT)
            {
                highlightSquare = new Point(-1, -1);
            }

            // Repaint
            Invalidate();
        }

        private void Board_MouseLeave(object sender, EventArgs e)
        {
            highlightSquare = new Point(-1, -1);
            mouseDownSquare = new Point(-1, -1);
            Invalidate();
        }

        private void Board_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDownSquare = highlightSquare;
            Invalidate();
        }

        private void Board_MouseUp(object sender, MouseEventArgs e)
        {
            if (tcs1 != null && mouseDownSquare.Equals(highlightSquare) && mouseDownSquare.X >= 0 && mouseDownSquare.Y >= 0 && mouseDownSquare.X < WIDTH && mouseDownSquare.Y < HEIGHT)
            {
                var tcs = tcs1;
                tcs1 = null;
                tcs.SetResult(new MoveDescriptor(mouseDownSquare.X, mouseDownSquare.Y));
            }

            mouseDownSquare = new Point(-1, -1);
        }

        public Task<MoveDescriptor> MakeMove(Board board, CancellationToken token)
        {
            this.board = board;
            tcs1 = new TaskCompletionSource<MoveDescriptor>();
            return tcs1.Task;
        }

        public Task OnMove(MoveDescriptor md)
        {
            RefreshBoard();
            return Task.FromResult(true);
        }
    }
}
