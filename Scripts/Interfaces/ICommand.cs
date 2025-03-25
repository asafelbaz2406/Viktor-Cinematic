using System.Threading.Tasks;

namespace Interfaces
{
    public interface ICommand
    {
        Task Execute();
    }
}