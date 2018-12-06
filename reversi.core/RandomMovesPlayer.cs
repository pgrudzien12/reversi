﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace reversi
{
    public class RandomMovesPlayer : IPlayerController
    {
        public Task<MoveDescriptor> MakeMove(Board board, CancellationToken cancellationToken)
        {
            return Task.FromResult(board.ValidMoves(board.currTurn)[0]);
        }

        public Task OnMove(Board b, MoveDescriptor md)
        {
            return Task.CompletedTask;
        }
    }
}
