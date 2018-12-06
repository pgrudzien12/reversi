using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace reversi
{
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

        public List<BoardTreeNode> GetChildren(int subDepth = 0)
        {
            if (children != null)
                return children;
            var list = new List<BoardTreeNode>();
            var validMoves = Board.ValidMoves(WhoWillMove);

            for (int i = 0; i < validMoves.Length; i++)
            {
                Board newBoard = Board.Clone();
                var whoMakesMove = Board.CurrTurn;
                newBoard.MakeMove(validMoves[i]);
                if (newBoard.LastPassed)
                {
                    var node = new BoardTreeNode(newBoard, whoMakesMove, newBoard.CurrTurn, validMoves[i], maxDepth);
                    if (subDepth == 6 || !node.GetChildren(subDepth + 1).Any())
                    {
                        node.Leaf = true;
                        list.Add(node);
                    }
                    else
                    {
                        foreach (var item in node.GetChildren())
                        {
                            item.MoveDescriptor = validMoves[i];
                            list.Add(item);
                        }
                    }

                }
                else
                {
                    var node = new BoardTreeNode(newBoard, whoMakesMove, newBoard.CurrTurn, validMoves[i], maxDepth);
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
            return children.OrderBy(c => c.Score).Last().MoveDescriptor;
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

            if (!children.Any())
            {
                Score = Board.Score(me);
                return;
            }

            if (WhoWillMove == me)
                Score = children.Min(c => c.Score);
            else
                Score = children.Max(c => c.Score);
        }
    }
}
