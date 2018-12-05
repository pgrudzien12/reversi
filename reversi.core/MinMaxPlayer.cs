using System.Threading;
using System.Threading.Tasks;

namespace reversi
{
    public class MinMaxPlayer : IPlayerController
    {
        public Task<MoveDescriptor> MakeMove(Board board, CancellationToken cancellationToken)
        {
            var me = board.currStatus.currTurn;

            MoveDescriptor bestMove;

            int bestScore = FindLocallyBestMove(board, me, 0, out bestMove);
            return Task.FromResult(bestMove);
        }

        private readonly int maxDepth;

        private int MinMax(Piece me, Piece whoMadeMove, Board board, int depth)
        {
            if (depth >= maxDepth || board.currStatus.gameEnded)
            {
                return board.Score(me) * (me == whoMadeMove ? 1 : -1);
            }

            MoveDescriptor bestMove;
            int bestScore = FindLocallyBestMove(board, me, depth, out bestMove);
            return bestScore;
        }

        public MinMaxPlayer(int maxDepth)
        {
            this.maxDepth = maxDepth;
        }

        private int FindLocallyBestMove(Board board, Piece me, int depth, out MoveDescriptor bestMove)
        {
            var validMoves = board.ValidMoves(me);
            bestMove = validMoves[0];
            int bestScore = int.MinValue;
            var myPoints = board.Score(me);
            for (int i = 0; i < validMoves.Length; i++)
            {
                Board newBoard = board.Clone();
                var whoMakesMove = board.currStatus.currTurn;
                newBoard.MakeMove(validMoves[i]);
                var score = MinMax(me, whoMakesMove, newBoard, depth + 1);
                if (score > bestScore)
                {
                    bestMove = validMoves[i];
                    bestScore = score;
                }
            }

            return bestScore;
        }

        public Task OnMove(MoveDescriptor md)
        {
            return Task.CompletedTask;
        }
    }
}
