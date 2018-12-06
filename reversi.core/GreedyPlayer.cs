using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace reversi
{
    public class GreedyPlayer : IPlayerController
    {
        public Task<MoveDescriptor> MakeMove(Board board, CancellationToken cancellationToken)
        {
            var me = board.CurrTurn;

            MoveDescriptor bestMove;
            int bestScore = FindLocallyBestMove(board, me, out bestMove);

            var expectedScore = bestScore;
            return Task.FromResult(bestMove);
        }

        private static int FindLocallyBestMove(Board board, Piece me, out MoveDescriptor bestMove)
        {
            var validMoves = board.ValidMoves(me);
            bestMove = validMoves[0];
            int bestScore = 0;
            var myPoints = board.Score(me);
            for (int i = 0; i < validMoves.Length; i++)
            {
                Board newBoard = board.Clone();
                newBoard.MakeMove(validMoves[i]);
                var score = newBoard.Score(me);
                if (score > bestScore)
                {
                    bestMove = validMoves[i];
                    bestScore = score;
                }
            }

            return bestScore;
        }

        public Task OnMove(Board b, MoveDescriptor md)
        {
            return Task.CompletedTask;
        }
    }
}
