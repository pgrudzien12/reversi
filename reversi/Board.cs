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
        { None = 0, Red = 1, Blue = 2 }

        /// <summary>The width of the board (must be at least 3)</summary>
        public const int WIDTH = 6;

        /// <summary>The height of the board (must be at least 3)</summary>
        public const int HEIGHT = 6;

        /// <summary>The current pieces on the board</summary>
        public Piece[,] pieces;

        /// <summary>Which square currently should be highlighted, or (-1, -1) if none should be highlighted</summary>
        private Point highlightSquare = new Point(-1, -1);

        /// <summary>The square where the left mouse button was pressed, or (-1, -1) if the left mouse button currently isn't down</summary>
        private Point mouseDownSquare = new Point(-1, -1);

        /// <summary>The form that a SquareClicked event handler should have</summary>
        /// <param name="square">The coordinates (in columns and rows) of the square that was clicked</param>
        public delegate void SquareClickedEventHandler(Point square);

        /// <summary>The event that is fired when a certain square is clicked</summary>
        public event SquareClickedEventHandler SquareClicked;

        /// <summary>Default constructor</summary>
        public Board()
        {
            // Initialize the control
            InitializeComponent();

            // Redraw when the control is resized
            ResizeRedraw = true;

            // Initialize the board
            ClearBoard();
        }

        /// <summary>Clears the board and places the initial pieces</summary>
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
        }

        /// <summary>Makes the given move for the given color</summary>
        /// <param name="col">The column where the piece should be placed</param>
        /// <param name="row">The row where the piece should be placed</param>
        /// <returns>Whether the move was valid or not</returns>
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
                    ;
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

            // If we've flipped over some pieces, the move was valid
            // In that case we only need to place the new piece
            // If we haven't flipped over any pieces, then nothing has changed
            // In that case we simply return false
            if(piecesFlipped)
            {
                pieces[col, row] = color;
                Invalidate();
                return true;
            }
            else
                return false;
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

            // Draw the highlighted square
            if(mouseDownSquare.X >= 0 && mouseDownSquare.Y >= 0 && mouseDownSquare.X < WIDTH && mouseDownSquare.Y < HEIGHT)
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
