using System.Threading.Tasks;
using Interfaces;

namespace Command_Pattern
{
    public abstract class ViktorCommand : ICommand
    {
        protected readonly IEntity Viktor;

        protected ViktorCommand(IEntity viktor)
        {
            Viktor = viktor;
        }

        public abstract Task Execute();

        public static T Create<T>(IEntity viktor) where T : ViktorCommand
        {
            return (T)System.Activator.CreateInstance(typeof(T), viktor);
        }
    }
}