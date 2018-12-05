using System.Threading.Tasks;

namespace reversi
{
    public interface IGameObserver
    {
        Task OnMove(Board board, MoveDescriptor md);
    }
}