using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace reversi
{
    public class MinMaxPlayer : IPlayerController
    {
        private int maxDepth;
        private BoardTreeNode node;

        public Task<MoveDescriptor> MakeMove(Board board, CancellationToken cancellationToken)
        {
            var me = board.currStatus.currTurn;
            if (node == null)
                node = new BoardTreeNode(board, Piece.None, me, null, maxDepth);
            return Task.FromResult(node.GetBestMove(me));
        }

        public MinMaxPlayer(int maxDepth)
        {
            this.maxDepth = maxDepth;
        }

        public Task OnMove(Board b, MoveDescriptor md)
        {
            if (node != null)
            {
                node = node.GetChildren().FirstOrDefault(n => n.MoveDescriptor == md && n.Board == b);
            }
            return Task.CompletedTask;
        }
    }
}
