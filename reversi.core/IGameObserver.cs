using System.Threading.Tasks;

namespace reversi
{
    public interface IGameObserver
    {
        Task OnMove(MoveDescriptor md);
    }
}