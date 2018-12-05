using System;
using System.Threading;
using System.Threading.Tasks;

namespace reversi
{
    public interface IPlayerController
    {
        Task<MoveDescriptor> MakeMove(Board board, CancellationToken cancellationToken);
    }
}