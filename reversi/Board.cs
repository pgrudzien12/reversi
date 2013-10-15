using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace reversi
{
    public partial class Board : Control
    {
        /// <summary>The different pieces, the piece is either red, blue or there is no piece</summary>
        public enum Piece : byte
        { Blue = 0, Red = 1, None = 2 }

        /// <summary>Describes the game status</summary>
        private struct GameStatus
        {
            /// <summary>Whose turn it currently is</summary>
            public Board.Piece currTurn;
            /// <summary>Whether or not the last player's turn was skipped because he couldn't move</summary>
            public bool lastPassed;
            /// <summary>Whether or not the game has ended</summary>
            public bool gameEnded;
            /// <summary>Whether or not to show hints</summary>
            public bool showHints;
        }

        /// <summary>The width of the board (must be at least 3)</summary>
        public const int WIDTH = 10;

        /// <summary>The height of the board (must be at least 3)</summary>
        public const int HEIGHT = 10;

        /// <summary>The current pieces on the board</summary>
        public Piece[,] pieces;

        /// <summary>The current status of the game</summary>
        private GameStatus currStatus = new GameStatus();

        /// <summary>Which square currently should be highlighted, or (-1, -1) if none should be highlighted</summary>
        private Point highlightSquare = new Point(-1, -1);

        /// <summary>The square where the left mouse button was pressed, or (-1, -1) if the left mouse button currently isn't down</summary>
        private Point mouseDownSquare = new Point(-1, -1);

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
        public Board()
        {
            // Initialize the control
            InitializeComponent();

            // Redraw when the control is resized
            this.ResizeRedraw = true;

            // Initialize the board
            ClearBoard();

            // Initially show the hints
            currStatus.showHints = true;
        }

        /// <summary>Clears the board, places the initial pieces and resets the game status</summary>
        public void ClearBoard()
        {
            // Create an array of pieces where all pieces are set to Piece.None
            pieces = new Piece[WIDTH, HEIGHT];
            for(int x = 0; x < WIDTH; ++x)
                for(int y = 0; y < HEIGHT; ++y)
                    pieces[x, y] = Piece.None;

            // Place the initial pieces in the middle of the board
            pieces[WIDTH / 2 - 1, HEIGHT / 2 - 1] = Piece.Blue;
            pieces[WIDTH / 2, HEIGHT / 2 - 1] = Piece.Red;
            pieces[WIDTH / 2 - 1, HEIGHT / 2] = Piece.Red;
            pieces[WIDTH / 2, HEIGHT / 2] = Piece.Blue;

            // Initialize a new game status
            currStatus.currTurn = Piece.Red;
            currStatus.gameEnded = false;
            currStatus.lastPassed = false;

            // Refresh the board
            RefreshBoard();
        }

        /// <summary>Refreshes the board, i.e. the board is redrawn and an UpdateStatus event is triggered</summary>
        public void RefreshBoard()
        {
            // Repaint
            Invalidate();

            // Trigger the UpdateStatus event
            if(UpdateStatus != null) UpdateStatus();
        }

        /// <summary>Gives the score of the given color (i.e. the amount of squares with that color)</summary>
        /// <param name="color">The color to count the squares for</param>
        /// <returns>The amount of squares with the given color</returns>
        public int Score(Piece color)
        {
            int score = 0;
            for(int col = 0; col < WIDTH; ++col)
            {
                for(int row = 0; row < HEIGHT; ++row)
                {
                    if(pieces[col, row] == color)
                        ++score;
                }
            }
            return score;
        }

        /// <summary>Whose turn it currently is</summary>
        public Piece CurrentTurn
        { get { return currStatus.currTurn; } }

        /// <summary>Whether or not the game has ended</summary>
        public bool GameEnded
        { get { return currStatus.gameEnded; } }

        /// <summary>Whether or not to show hints</summary>
        public bool ShowHints
        {
            get { return currStatus.showHints; }
            set
            {
                currStatus.showHints = value;
                RefreshBoard();
            }
        }

        /// <summary>Whether or not the last player has passed</summary>
        public bool LastPassed
        { get { return currStatus.lastPassed; } }

        /// <summary>Returns all valid moves for a player</summary>
        /// <param name="color">The player whose moves we have to check</param>
        /// <returns>An array with all the valid moves for the player, empty if no moves possible</returns>
        public Point[] ValidMoves(Piece color)
        {
            List<Point> Moves = new List<Point>();
            for(int col = 0; col < WIDTH; ++col)
            {
                for(int row = 0; row < HEIGHT; ++row)
                {
                    // Making no move at all is always invalid
                    if(color == Piece.None) continue;

                    // Check if `col` and `row` are in the boundaries of the board and if (`col`, `row`) is an empty square
                    if(pieces[col, row] != Piece.None) continue;

                    // Flip over the pieces of the other color that become enclosed between two pieces of `color`
                    bool piecesFlipped = false;                 // Whether or not some pieces are flipped over
                    for(int dx = -1; dx <= 1; ++dx)
                    {
                        for(int dy = -1; dy <= 1; ++dy)
                        {
                            if(dx == 0 && dy == 0) continue;

                            // Determine the amount of steps that we should go in the current direction until we encounter a piece of our own color
                            // Then, if we find a piece of our own color, flip over all pieces in between
                            // If we do encounter such a piece, or if we encounter an empty square first, we won't flip over any pieces
                            for(int steps = 1; steps <= Math.Max(WIDTH, HEIGHT); ++steps)
                            {
                                int currX = col + steps * dx;
                                int currY = row + steps * dy;
                                if(currX < 0 || currX >= WIDTH || currY < 0 || currY >= HEIGHT || pieces[currX, currY] == Piece.None)
                                    break;

                                if(pieces[currX, currY] == color)
                                {
                                    piecesFlipped = piecesFlipped || steps > 1;
                                    break;
                                }
                            }
                        }
                    }

                    // If we've flipped over some pieces, the move was valid
                    // In that case we only need to place the new piece
                    // If we haven't flipped over any pieces, then nothing has changed
                    // In that case we simply return false
                    if(piecesFlipped)
                        Moves.Add(new Point(col, row));
                    else
                        continue;
                }
            }

            return Moves.ToArray();
        }

        /// <summary>Makes the given move for the color whose turn it currently is</summary>
        /// <param name="col">The column where the piece should be placed</param>
        /// <param name="row">The row where the piece should be placed</param>
        /// <returns>Whether the move succeeded or not</returns>
        public bool MakeMove(int col, int row)
        { return MakeMove(col, row, currStatus.currTurn); }

        /// <summary>Makes the given move for the given color</summary>
        /// <param name="col">The column where the piece should be placed</param>
        /// <param name="row">The row where the piece should be placed</param>
        /// <param name="color">The color whose move it is</param>
        /// <returns>Whether the move succeeded or not</returns>
        public bool MakeMove(int col, int row, Piece color)
        {
            // Making no move at all is always invalid
            if(color == Piece.None) return false;

            // Check if `col` and `row` are in the boundaries of the board and if (`col`, `row`) is an empty square
            if(col < 0 || row < 0 || col >= WIDTH || row >= HEIGHT || pieces[col, row] != Piece.None)
                return false;

            // Flip over the pieces of the other color that become enclosed between two pieces of `color`
            bool piecesFlipped = false;                 // Whether or not some pieces are flipped over
            for(int dx = -1; dx <= 1; ++dx)
            {
                for(int dy = -1; dy <= 1; ++dy)
                {
                    if(dx == 0 && dy == 0) continue;

                    // Determine the amount of steps that we should go in the current direction until we encounter a piece of our own color
                    // Then, if we find a piece of our own color, flip over all pieces in between
                    // If we do encounter such a piece, or if we encounter an empty square first, we won't flip over any pieces
                    for(int steps = 1; steps <= Math.Max(WIDTH, HEIGHT); ++steps)
                    {
                        int currX = col + steps * dx;
                        int currY = row + steps * dy;
                        if(currX < 0 || currX >= WIDTH || currY < 0 || currY >= HEIGHT || pieces[currX, currY] == Piece.None)
                            break;

                        if(pieces[currX, currY] == color)
                        {
                            if(steps > 1)
                                piecesFlipped = true;
                            for(int i = 1; i < steps; ++i)
                                pieces[col + i * dx, row + i * dy] = color;
                            break;
                        }
                    }
                }
            }

            // If no pieces were flipped, the move wasn't valid
            if(!piecesFlipped) return false;

            // Now we only need to place the new piece
            pieces[col, row] = color;

            // If the next player can't play, let him skip the turn
            if(ValidMoves((currStatus.currTurn == Piece.Red ? Piece.Blue : Piece.Red)).Length == 0)
            {
                // Check if the game has ended
                if(ValidMoves(currStatus.currTurn).Length == 0)
                    currStatus.gameEnded = true;
                else
                    currStatus.lastPassed = true;
            }
            else
            {
                currStatus.currTurn = currStatus.currTurn == Piece.Red ? Piece.Blue : Piece.Red;
                currStatus.lastPassed = false;
            }

            // Refresh the board
            RefreshBoard();

            // Since we've come here, the move must have been valid
            return true;
        }

        /// <summary>Draws a smileys that looks towards the mouse</summary>
        /// <param name="g">The graphics object that's used to draw the smiley</param>
        /// <param name="color">The color to use for the background of the smiley</param>
        /// <param name="x">The x-coordinate of the top-left corner of the smiley</param>
        /// <param name="y">The y-coordinate of the top-left corner of the smiley</param>
        /// <param name="size">The size (diameter) of the smiley</param>
        private void drawSmiley(Graphics g, Brush color, int x, int y, int size)
        {
            // If the size is zero (or smaller), there is nothing to do
            if(size <= 0.0f) return;
            
            // Draw the background of the smiley
            g.FillEllipse(color, x, y, size, size);

            // Draw the mouth
            GraphicsPath mouthPath = new GraphicsPath();
            mouthPath.StartFigure();
            mouthPath.AddArc(x + 5.0f * size / 32, y + 3.0f * size / 32.0f, 11.0f * size / 16, 13.0f * size / 16, 0.0f, 180.0f);
            mouthPath.CloseFigure();
            g.FillPath(Brushes.White, mouthPath);
            g.DrawPath(Pens.Black, mouthPath);

            // Draw the eyes
            drawSmileyEye(g, x + 5.0f * size / 32, y + size / 8.0f, 5.0f * size / 16);
            drawSmileyEye(g, x + 17.0f * size / 32, y + size / 8.0f, 5.0f * size / 16);
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
            mouseCoords.X -= (int) g.Transform.OffsetX;
            mouseCoords.Y -= (int) g.Transform.OffsetY;

            // Determine the center x and center y of the smiley
            float centerX = x + size / 2;
            float centerY = y + size / 2;

            // Draw the background of the eye
            g.FillEllipse(Brushes.White, x, y, size, size);
            g.DrawEllipse(Pens.Black, x, y, size, size);

            // Draw the pupil
            float pupilRadius = size / 10;
            float dst = (float) Math.Sqrt(Math.Pow(mouseCoords.X - centerX, 2) + Math.Pow(mouseCoords.Y - centerY, 2));
            if(dst <= size / 2 - pupilRadius)
                g.FillEllipse(Brushes.Black, mouseCoords.X - pupilRadius, mouseCoords.Y - pupilRadius, pupilRadius * 2, pupilRadius * 2);
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
            int squareSize = (int) Math.Min(Width / WIDTH, Height / HEIGHT);
            pea.Graphics.TranslateTransform((Width - WIDTH * squareSize) / 2, (Height - HEIGHT * squareSize) / 2);

            // If the control is too small, we stop here
            if(squareSize == 0) return;

            // Draw the highlighted square, but only if the game hasn't ended yet
            if(currStatus.gameEnded)
                Cursor = Cursors.Default;
            else if(mouseDownSquare.X >= 0 && mouseDownSquare.Y >= 0 && mouseDownSquare.X < WIDTH && mouseDownSquare.Y < HEIGHT)
            {
                if(pieces[mouseDownSquare.X, mouseDownSquare.Y] == Piece.None)
                {
                    pea.Graphics.FillRectangle(Brushes.Black, mouseDownSquare.X * squareSize, mouseDownSquare.Y * squareSize, squareSize, squareSize);
                    Cursor = Cursors.Hand;
                }
                else
                    Cursor = Cursors.Default;
            }
            else if(highlightSquare.X >= 0 && highlightSquare.Y >= 0 && highlightSquare.X < WIDTH && highlightSquare.Y < HEIGHT && pieces[highlightSquare.X, highlightSquare.Y] == Piece.None)
            {
                pea.Graphics.FillRectangle(Brushes.DarkGray, highlightSquare.X * squareSize, highlightSquare.Y * squareSize, squareSize, squareSize);
                Cursor = Cursors.Hand;
            }
            else
                Cursor = Cursors.Default;

            // Draw the board
            for(int i = 0; i < WIDTH; ++i)
                pea.Graphics.DrawLine(Pens.Black, i * squareSize, 0, i * squareSize, HEIGHT * squareSize - 1);
            pea.Graphics.DrawLine(Pens.Black, WIDTH * squareSize - 1, 0, WIDTH * squareSize - 1, HEIGHT * squareSize - 1);
            for(int i = 0; i < HEIGHT; ++i)
                pea.Graphics.DrawLine(Pens.Black, 0, i * squareSize, WIDTH * squareSize - 1, i * squareSize);
            pea.Graphics.DrawLine(Pens.Black, 0, HEIGHT * squareSize - 1, WIDTH * squareSize - 1, HEIGHT * squareSize - 1);

            // Draw the pieces
            for(int x = 0; x < WIDTH; ++x)
            {
                for(int y = 0; y < HEIGHT; ++y)
                {
                    if(pieces[x, y] == Piece.None) continue;

                    drawSmiley(pea.Graphics, pieces[x, y] == Piece.Blue ? Brushes.Blue : Brushes.Red, x * squareSize + 1, y * squareSize + 1, squareSize - 2);
                }
            }

            // Draw the hints (all possible moves)
            if(currStatus.showHints)
            {
                Point[] validMoves = ValidMoves(currStatus.currTurn);
                foreach(Point validMove in validMoves)
                    drawSmiley(pea.Graphics, Brushes.Transparent, validMove.X * squareSize + 4, validMove.Y * squareSize + 4, squareSize - 8);
            }
        }

        private void Board_MouseMove(object sender, MouseEventArgs mea)
        {
            int squareSize = (int) Math.Min(Width / WIDTH, Height / HEIGHT);    // Calculate the size of a square
            int transX = (Width - WIDTH * squareSize) / 2;                      // The horizontal translation of the board
            int transY = (Height - HEIGHT * squareSize) / 2;                    // The vertical translation of the board

            // If the control is too small, we stop here
            if(squareSize == 0) return;

            // Calculate the square that we're hovering above and redraw the board
            highlightSquare = new Point((mea.X - transX) / squareSize, (mea.Y - transY) / squareSize);
            if(mea.X < transX || mea.Y < transY || highlightSquare.X < 0 || highlightSquare.Y < 0 || highlightSquare.X >= WIDTH || highlightSquare.Y >= HEIGHT)
                highlightSquare = new Point(-1, -1);
            
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
            if(SquareClicked != null && mouseDownSquare.Equals(highlightSquare) && mouseDownSquare.X >= 0 && mouseDownSquare.Y >= 0 && mouseDownSquare.X < WIDTH && mouseDownSquare.Y < HEIGHT)
                SquareClicked(mouseDownSquare);
            mouseDownSquare = new Point(-1, -1);
            Invalidate();
        }
    }
}
