using System;
using System.Collections.Generic;

namespace reversi
{
    public class Board
    {

        /// <summary>The width of the board (must be at least 3)</summary>
        public const int WIDTH = 8;

        /// <summary>The height of the board (must be at least 3)</summary>
        public const int HEIGHT = 8;

        /// <summary>The current pieces on the board</summary>
        private Piece[,] pieces;

        /// <summary>The current status of the game</summary>
        public GameStatus currStatus = new GameStatus();
        public Piece this[int x, int y]
        {
            get
            {
                return pieces[x, y];
            }
        }

        internal Board Clone()
        {
            var clone = new Board();
            clone.currStatus.currTurn = this.currStatus.currTurn;
            clone.currStatus.gameEnded = this.currStatus.gameEnded;
            clone.currStatus.lastPassed = this.currStatus.lastPassed;

            clone.pieces = new Piece[WIDTH, HEIGHT];
            for (int x = 0; x < WIDTH; ++x)
            {
                for (int y = 0; y < HEIGHT; ++y)
                {
                    clone.pieces[x, y] = pieces[x, y];
                }
            }
            return clone;
        }


        /// <summary>Makes the given move for the color whose turn it currently is</summary>
        /// <param name="col">The column where the piece should be placed</param>
        /// <param name="row">The row where the piece should be placed</param>
        /// <returns>Whether the move succeeded or not</returns>
        public bool MakeMove(int col, int row)
        {
            return MakeMove(col, row, currStatus.currTurn);
        }


        public bool MakeMove(MoveDescriptor md)
        {
            return MakeMove(md.X, md.Y);
        }

        /// <summary>Makes the given move for the given color</summary>
        /// <param name="col">The column where the piece should be placed</param>
        /// <param name="row">The row where the piece should be placed</param>
        /// <param name="color">The color whose move it is</param>
        /// <returns>Whether the move succeeded or not</returns>
        public bool MakeMove(int col, int row, Piece color)
        {
            // Making no move at all is always invalid
            if (color == Piece.None)
            {
                return false;
            }

            // Check if `col` and `row` are in the boundaries of the board and if (`col`, `row`) is an empty square
            if (col < 0 || row < 0 || col >= WIDTH || row >= HEIGHT || pieces[col, row] != Piece.None)
            {
                return false;
            }

            // Flip over the pieces of the other color that become enclosed between two pieces of `color`
            bool piecesFlipped = false;                 // Whether or not some pieces are flipped over
            for (int dx = -1; dx <= 1; ++dx)
            {
                for (int dy = -1; dy <= 1; ++dy)
                {
                    if (dx == 0 && dy == 0)
                    {
                        continue;
                    }

                    // Determine the amount of steps that we should go in the current direction until we encounter a piece of our own color
                    // Then, if we find a piece of our own color, flip over all pieces in between
                    // If we do encounter such a piece, or if we encounter an empty square first, we won't flip over any pieces
                    for (int steps = 1; steps <= Math.Max(WIDTH, HEIGHT); ++steps)
                    {
                        int currX = col + steps * dx;
                        int currY = row + steps * dy;
                        if (currX < 0 || currX >= WIDTH || currY < 0 || currY >= HEIGHT || pieces[currX, currY] == Piece.None)
                        {
                            break;
                        }

                        if (pieces[currX, currY] == color)
                        {
                            if (steps > 1)
                            {
                                piecesFlipped = true;
                            }

                            for (int i = 1; i < steps; ++i)
                            {
                                pieces[col + i * dx, row + i * dy] = color;
                            }

                            break;
                        }
                    }
                }
            }

            // If no pieces were flipped, the move wasn't valid
            if (!piecesFlipped)
            {
                return false;
            }

            // Now we only need to place the new piece
            pieces[col, row] = color;

            // If the next player can't play, let him skip the turn
            if (ValidMoves((currStatus.currTurn == Piece.Red ? Piece.Blue : Piece.Red)).Length == 0)
            {
                // Check if the game has ended
                if (ValidMoves(currStatus.currTurn).Length == 0)
                {
                    currStatus.gameEnded = true;
                }
                else
                {
                    currStatus.lastPassed = true;
                }
            }
            else
            {
                currStatus.currTurn = currStatus.currTurn == Piece.Red ? Piece.Blue : Piece.Red;
                currStatus.lastPassed = false;
            }

            // Since we've come here, the move must have been valid
            return true;
        }

        public void ClearBoard()
        {
            // Create an array of pieces where all pieces are set to Piece.None
            pieces = new Piece[WIDTH, HEIGHT];
            for (int x = 0; x < WIDTH; ++x)
            {
                for (int y = 0; y < HEIGHT; ++y)
                {
                    pieces[x, y] = Piece.None;
                }
            }

            // Place the initial board.pieces in the middle of the board
            pieces[WIDTH / 2 - 1, HEIGHT / 2 - 1] = Piece.Blue;
            pieces[WIDTH / 2, HEIGHT / 2 - 1] = Piece.Red;
            pieces[WIDTH / 2 - 1, HEIGHT / 2] = Piece.Red;
            pieces[WIDTH / 2, HEIGHT / 2] = Piece.Blue;

            // Initialize a new game status
            currStatus.currTurn = Piece.Red;
            currStatus.gameEnded = false;
            currStatus.lastPassed = false;
        }


        /// <summary>Returns all valid moves for a player</summary>
        /// <param name="color">The player whose moves we have to check</param>
        /// <returns>An array with all the valid moves for the player, empty if no moves possible</returns>
        public MoveDescriptor[] ValidMoves(Piece color)
        {
            List<MoveDescriptor> Moves = new List<MoveDescriptor>();
            for (int col = 0; col < WIDTH; ++col)
            {
                for (int row = 0; row < HEIGHT; ++row)
                {
                    // Making no move at all is always invalid
                    if (color == Piece.None)
                    {
                        continue;
                    }

                    // Check if `col` and `row` are in the boundaries of the board and if (`col`, `row`) is an empty square
                    if (pieces[col, row] != Piece.None)
                    {
                        continue;
                    }

                    // Flip over the board.pieces of the other color that become enclosed between two board.pieces of `color`
                    bool piecesFlipped = false;                 // Whether or not some pieces are flipped over
                    for (int dx = -1; dx <= 1; ++dx)
                    {
                        for (int dy = -1; dy <= 1; ++dy)
                        {
                            if (dx == 0 && dy == 0)
                            {
                                continue;
                            }

                            // Determine the amount of steps that we should go in the current direction until we encounter a piece of our own color
                            // Then, if we find a piece of our own color, flip over all pieces in between
                            // If we do encounter such a piece, or if we encounter an empty square first, we won't flip over any pieces
                            for (int steps = 1; steps <= Math.Max(WIDTH, HEIGHT); ++steps)
                            {
                                int currX = col + steps * dx;
                                int currY = row + steps * dy;
                                if (currX < 0 || currX >= WIDTH || currY < 0 || currY >= HEIGHT || pieces[currX, currY] == Piece.None)
                                {
                                    break;
                                }

                                if (pieces[currX, currY] == color)
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
                    if (piecesFlipped)
                    {
                        Moves.Add(new MoveDescriptor(col, row));
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            return Moves.ToArray();
        }

        /// <summary>Gives the score of the given color (i.e. the amount of squares with that color)</summary>
        /// <param name="color">The color to count the squares for</param>
        /// <returns>The amount of squares with the given color</returns>
        public int Score(Piece color)
        {
            int score = 0;
            for (int col = 0; col < WIDTH; ++col)
            {
                for (int row = 0; row < HEIGHT; ++row)
                {
                    if (pieces[col, row] == color)
                    {
                        ++score;
                    }
                }
            }
            return score;
        }
    }
    public class MoveDescriptor
    {
        public MoveDescriptor(int col, int row)
        {
            X = col;
            Y = row;
        }

        public int X { get; set; }
        public int Y { get; set; }
    }
}