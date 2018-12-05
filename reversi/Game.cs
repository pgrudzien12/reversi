using System.Threading;
using System.Threading.Tasks;

namespace reversi
{
    public class Game
    {
        private readonly IPlayerController _playerRed;
        private readonly IPlayerController _playerBlue;
        private CancellationToken cancellationToken;

        public Game(IPlayerController playerRed, IPlayerController playerBlue)
        {
            _playerRed = playerRed;
            _playerBlue = playerBlue;
        }

        public IPlayerController CurrentPlayer => Board.currStatus.currTurn == Piece.Blue ? _playerBlue : _playerRed;

        public Board Board { get; set; } = new Board();

        public async Task PlayAsync()
        {
            cancellationToken = new CancellationToken();
            Board.ClearBoard();
            while (true)
            {
                var md = await CurrentPlayer.MakeMove(Board, cancellationToken);
                if (Board.MakeMove(md))
                {
                    await _playerBlue.OnMove(md);
                    await _playerBlue.OnMove(md);
                }
                if (Board.currStatus.gameEnded)
                {
                    break;
                }
            }
        }
    }
}
