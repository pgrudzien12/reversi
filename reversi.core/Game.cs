using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace reversi
{
    public class Game
    {
        private readonly IList<IGameObserver> _observers = new List<IGameObserver>();
        private readonly IPlayerController _playerRed;
        private readonly IPlayerController _playerBlue;
        private CancellationToken cancellationToken;

        public Game(IPlayerController playerRed, IPlayerController playerBlue)
        {
            _playerRed = playerRed;
            _playerBlue = playerBlue;
            _observers.Add(_playerRed);
            _observers.Add(_playerBlue);
        }

        public IPlayerController CurrentPlayer => Board.currStatus.currTurn == Piece.Blue ? _playerBlue : _playerRed;

        public void AddObserver(IGameObserver observer)
        {
            _observers.Add(observer);
        }

        public Board Board { get; set; } = new Board();

        public async Task PlayAsync()
        {
            cancellationToken = new CancellationToken();
            await ResetGame();
            while (true)
            {
                var md = await CurrentPlayer.MakeMove(Board.Clone(), cancellationToken);
                if (Board.MakeMove(md))
                {
                    await NotifyObserversOnMove(md);
                }
                if (Board.currStatus.gameEnded)
                {
                    break;
                }
            }
        }

        public Task ResetGame()
        {
            Board.ClearBoard();
            return NotifyObserversOnClean();
        }

        private async Task NotifyObserversOnMove(MoveDescriptor md)
        {
            foreach (var o in _observers)
            {
                await o.OnMove(Board, md);
            }
        }

        private async Task NotifyObserversOnClean()
        {
            foreach (var o in _observers)
            {
                await o.OnMove(Board, default(MoveDescriptor));
            }
        }
    }
}
