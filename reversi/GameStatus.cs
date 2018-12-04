namespace reversi
{
    /// <summary>Describes the game status</summary>
    public struct GameStatus
    {
        /// <summary>Whose turn it currently is</summary>
        public Piece currTurn;
        /// <summary>Whether or not the last player's turn was skipped because he couldn't move</summary>
        public bool lastPassed;
        /// <summary>Whether or not the game has ended</summary>
        public bool gameEnded;

        /// <summary>Whether or not to show hints</summary>
        public bool showHints;


    }
}
