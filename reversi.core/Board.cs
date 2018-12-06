using System;
using System.Collections.Generic;
using System.Text;

namespace reversi
{
    public class Board : IEquatable<Board>
    {
        /// <summary>The width of the board (don't change)</summary>
        private const byte WIDTH = 8;

        /// <summary>The height of the board (dont' change)</summary>
        private const byte HEIGHT = 8;

        /// <summary>The current pieces on the board</summary>
        private Vector128 pieces;
        private int redPoints;
        private int bluePoints;

        public Piece this[byte pos]
        {
            get
            {
                return (Piece)(pieces[pos] + pieces[(byte)(pos + 8)]);
            }
            set
            {
                var pos8 = (byte)(pos + 8);
                if (pieces[pos] == 1)
                {
                    if (pieces[pos8] == 0)
                    {
                        redPoints--;
                    }
                    else
                    {
                        bluePoints--;
                    }
                }
                if (value == Piece.None)
                {
                    pieces[pos] = 0;
                    pieces[pos8] = 0;
                }
                else if (value == Piece.Red)
                {
                    pieces[pos] = 1;
                    pieces[pos8] = 0;
                    redPoints++;
                }
                else if (value == Piece.Blue)
                {
                    pieces[pos] = 1;
                    pieces[pos8] = 1;
                    bluePoints++;
                }
            }
        }

        public Piece this[int x, int y]
        {
            get
            {
                return this[(byte)x, (byte)y];
            }
            set
            {
                this[(byte)x, (byte)y] = value;
            }
        }

        public Piece this[byte x, byte y]
        {
            get
            {
                var pos = (byte)((y << 4) + x);
                return this[pos];
            }
            private set
            {
                var pos = (byte)((y << 4) + x);
                this[pos] = value;
            }
        }

        internal Board Clone()
        {
            var clone = new Board();
            clone.CurrTurn = CurrTurn;
            clone.GameEnded = GameEnded;
            clone.LastPassed = LastPassed;
            clone.bluePoints = bluePoints;
            clone.redPoints = redPoints;

            clone.pieces = pieces;
            return clone;
        }

        private static Vector128 NewBoard()
        {
            return new Vector128();
        }


        /// <summary>Makes the given move for the color whose turn it currently is</summary>
        /// <param name="col">The column where the piece should be placed</param>
        /// <param name="row">The row where the piece should be placed</param>
        /// <returns>Whether the move succeeded or not</returns>
        public bool MakeMove(MoveDescriptor md)
        {
            return MakeMove(md, CurrTurn);
        }

        /// <summary>Makes the given move for the given color</summary>
        /// <param name="col">The column where the piece should be placed</param>
        /// <param name="row">The row where the piece should be placed</param>
        /// <param name="color">The color whose move it is</param>
        /// <returns>Whether the move succeeded or not</returns>
        public bool MakeMove(MoveDescriptor md, Piece color)
        {
            // Making no move at all is always invalid
            if (color == Piece.None)
            {
                return false;
            }

            // Check if `col` and `row` are in the boundaries of the board and if (`col`, `row`) is an empty square
            if ((md.Position & 0x88) > 0 || pieces[md.Position] != 0)
            {
                return false;
            }

            int colorNo = color == Piece.Red ? 0 : 1;
            // Flip over the pieces of the other color that become enclosed between two pieces of `color`
            bool piecesFlipped = false;                 // Whether or not some pieces are flipped over

            for (int j = 0; j < bishopMoves.Length; j++)
            {
                byte currPos = md.Position;
                byte currMove = bishopMoves[j];
                // Determine the amount of steps that we should go in the current direction until we encounter a piece of our own color
                // Then, if we find a piece of our own color, flip over all pieces in between
                // If we do encounter such a piece, or if we encounter an empty square first, we won't flip over any pieces
                for (int steps = 1; steps <= 7; ++steps)
                {
                    currPos += currMove;
                    if (((currPos & 0x88) > 0) || pieces[currPos] == 0)
                    {
                        break;
                    }

                    if (pieces[(byte)(currPos + 8)] == colorNo)
                    {
                        if (steps > 1)
                        {
                            piecesFlipped = true;
                        }
                        byte innerPos = md.Position;
                        for (int i = 1; i < steps; ++i)
                        {
                            innerPos += currMove;
                            this[innerPos] = color;
                        }

                        break;
                    }

                }
            }

            // If no pieces were flipped, the move wasn't valid
            if (!piecesFlipped)
            {
                return false;
            }

            // Now we only need to place the new piece
            this[md.Position] = color;

            // If the next player can't play, let him skip the turn
            if (ValidMoves((CurrTurn == Piece.Red ? Piece.Blue : Piece.Red), true).Length == 0)
            {
                // Check if the game has ended
                if (ValidMoves(CurrTurn, true).Length == 0)
                {
                    GameEnded = true;
                }
                else
                {
                    LastPassed = true;
                }
            }
            else
            {
                CurrTurn = CurrTurn == Piece.Red ? Piece.Blue : Piece.Red;
                LastPassed = false;
            }

            // Since we've come here, the move must have been valid
            return true;
        }

        public void ClearBoard()
        {
            // Create an array of pieces where all pieces are set to Piece.None
            pieces = NewBoard();
            bluePoints = 0;
            redPoints = 0;
            // Place the initial board.pieces in the middle of the board
            this[WIDTH / 2 - 1, HEIGHT / 2 - 1] = Piece.Blue;
            this[WIDTH / 2, HEIGHT / 2 - 1] = Piece.Red;
            this[WIDTH / 2 - 1, HEIGHT / 2] = Piece.Red;
            this[WIDTH / 2, HEIGHT / 2] = Piece.Blue;

            // Initialize a new game status
            CurrTurn = Piece.Red;
            GameEnded = false;
            LastPassed = false;
        }

        static Board()
        {
            unchecked
            {
                bishopMoves = new byte[] { 17, 16, 15, (byte)-17, (byte)-16, (byte)-15, (byte)-1, 1 };
            }
        }

        private static byte[] bishopMoves;

        public Piece CurrTurn { get; set; }
        public bool LastPassed { get; set; }
        public bool GameEnded { get; set; }
        private static MoveDescriptor[] ZeroDescriptoArray = new MoveDescriptor[0];
        private static MoveDescriptor[] NonZeroDescriptoArray = new MoveDescriptor[1];

        /// <summary>Returns all valid moves for a player</summary>
        /// <param name="color">The player whose moves we have to check</param>
        /// <returns>An array with all the valid moves for the player, empty if no moves possible</returns>
        public MoveDescriptor[] ValidMoves(Piece color, bool stopOnFirst = false)
        {
            // Making no move at all is always invalid
            if (color == Piece.None)
            {
                return ZeroDescriptoArray;
            }

            int colorNo = color == Piece.Red ? 0 : 1;
            List<MoveDescriptor> moves = null;
            if (!stopOnFirst)
                moves = new List<MoveDescriptor>();
            for (byte col = 0; col < WIDTH; ++col)
            {
                for (byte row = 0; row < HEIGHT; ++row)
                {
                    byte pos = (byte)((row << 4) + col);
                    // Check if `col` and `row` are in the boundaries of the board and if (`col`, `row`) is an empty square
                    if (pieces[pos] != 0)
                    {
                        continue;
                    }

                    // Flip over the board.pieces of the other color that become enclosed between two board.pieces of `color`
                    bool piecesFlipped = false;                 // Whether or not some pieces are flipped over
                    for (int i = 0; i < bishopMoves.Length; i++)
                    {
                        byte currPos = pos;
                        // Determine the amount of steps that we should go in the current direction until we encounter a piece of our own color
                        // Then, if we find a piece of our own color, flip over all pieces in between
                        // If we do encounter such a piece, or if we encounter an empty square first, we won't flip over any pieces
                        for (int steps = 1; steps <= 8; ++steps)
                        {
                            currPos = (byte)(currPos + bishopMoves[i]);
                            if (((currPos & 0x88) > 0) || pieces[currPos] == 0)
                            {
                                break;
                            }
                            if (pieces[(byte)(currPos + 8)] == colorNo)
                            {
                                piecesFlipped = piecesFlipped || steps > 1;
                                break;
                            }
                        }
                    }

                    // If we've flipped over some pieces, the move was valid
                    // In that case we only need to place the new piece
                    // If we haven't flipped over any pieces, then nothing has changed
                    // In that case we simply return false
                    if (piecesFlipped)
                    {
                        if (stopOnFirst)
                        {
                            return NonZeroDescriptoArray;
                        }
                        moves.Add(new MoveDescriptor(pos));
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            if (stopOnFirst)
            {
                return ZeroDescriptoArray;
            }
            return moves.ToArray();
        }

        /// <summary>Gives the score of the given color (i.e. the amount of squares with that color)</summary>
        /// <param name="color">The color to count the squares for</param>
        /// <returns>The amount of squares with the given color</returns>
        public int Score(Piece color)
        {
            return color == Piece.Red ? redPoints : bluePoints;
        }

        public override bool Equals(object other)
        {
            return this.Equals(other as Board);
        }

        public bool Equals(Board other)
        {
            if (other == null)
            {
                return false;
            }

            return pieces == other.pieces;
        }

        public override int GetHashCode()
        {
            return -530229870 + EqualityComparer<Vector128>.Default.GetHashCode(pieces);
        }

        public static bool operator ==(Board board1, Board board2)
        {
            return EqualityComparer<Board>.Default.Equals(board1, board2);
        }

        public static bool operator !=(Board board1, Board board2)
        {
            return !(board1 == board2);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (byte y = 0; y < HEIGHT; y++)
            {
                for (byte x = 0; x < WIDTH; x++)
                {
                    char c = '_';
                    switch (this[x, y])
                    {
                        case Piece.Blue:
                            c = '0';
                            break;
                        case Piece.Red:
                            c = 'x';
                            break;
                        default:
                            break;
                    }
                    sb.Append(c);
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
    public struct MoveDescriptor
    {
        public MoveDescriptor(byte pos)
        {
            Position = pos;
        }

        public MoveDescriptor(int col, int row)
        {
            Position = (byte)((row << 4) + col);
        }

        public byte X => (byte)(Position & 7);
        public byte Y => (byte)(Position >> 4 & 7);
        public byte Position { get; }

        public override bool Equals(object obj)
        {
            if (!(obj is MoveDescriptor))
            {
                return false;
            }

            var descriptor = (MoveDescriptor)obj;
            return Position == descriptor.Position;
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode();
        }

        public static bool operator ==(MoveDescriptor descriptor1, MoveDescriptor descriptor2)
        {
            return descriptor1.Equals(descriptor2);
        }

        public static bool operator !=(MoveDescriptor descriptor1, MoveDescriptor descriptor2)
        {
            return !(descriptor1 == descriptor2);
        }
    }
}