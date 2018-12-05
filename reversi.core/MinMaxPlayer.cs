using System;
using System.Collections.Generic;
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
            if (node != null)
            {
                node = node.GetChildren().FirstOrDefault(n => n.Board == board && n.Board.currStatus.currTurn == me);
            }
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

    public class BoardTreeNode
    {
        public int maxDepth;
        public BoardTreeNode(Board board, Piece whoMakesMove, Piece whoWillMove, MoveDescriptor moveDescriptor, int maxDepth)
        {
            Board = board;
            WhoMadeMove = whoMakesMove;
            WhoWillMove = whoWillMove;
            MoveDescriptor = moveDescriptor;
            this.maxDepth = maxDepth;
        }

        public Board Board { get; }
        public Piece WhoMadeMove { get; }
        public Piece WhoWillMove { get; }
        public MoveDescriptor MoveDescriptor { get; set; }
        public int Score { get; private set; }
        public bool Leaf { get; private set; }

        private List<BoardTreeNode> children = null;

        public List<BoardTreeNode> GetChildren()
        {
            if (children != null)
                return children;

            var list = new List<BoardTreeNode>();
            var validMoves = Board.ValidMoves(WhoWillMove);
                
            for (int i = 0; i < validMoves.Length; i++)
            {
                Board newBoard = Board.Clone();
                var whoMakesMove = Board.currStatus.currTurn;
                newBoard.MakeMove(validMoves[i]);
                if (newBoard.currStatus.lastPassed)
                {
                    var node = new BoardTreeNode(newBoard, whoMakesMove, newBoard.currStatus.currTurn, validMoves[i], maxDepth);
                    if (!node.GetChildren().Any())
                    {
                        node.Leaf = true;
                        list.Add(node);
                    }
                    foreach (var item in node.GetChildren())
                    {
                        item.MoveDescriptor = validMoves[i];
                        list.Add(item);
                    }
                    
                }
                else
                {
                    var node = new BoardTreeNode(newBoard, whoMakesMove, newBoard.currStatus.currTurn, validMoves[i], maxDepth);
                    list.Add(node);
                }
            }
            children = list;
            return list;
        }

        public MoveDescriptor GetBestMove(Piece me, int depth = 0)
        {
            var children = GetChildren();
            foreach (var item in children)
            {
                item.CalculateScore(me, depth);
            }
            return children.OrderBy(c => c.Score).First().MoveDescriptor;
        }

        private void CalculateScore(Piece me, int depth)
        {
            if (depth == maxDepth || Leaf)
            {
                Score = Board.Score(me);
                return;
            }

            var children = GetChildren();
            foreach (var item in children)
            {
                item.CalculateScore(me, depth + 1);
            }

            IEnumerable<BoardTreeNode> ordered = children.OrderBy(c => c.Score); // max
            if (WhoWillMove != me)
                ordered = ordered.Reverse(); // min
            if (!ordered.Any())
            {
                Score = Board.Score(me);
                return;
            }
            Score = ordered.First().Score;
        }
    }
}
