using System.Collections.Generic;
using System.Threading.Tasks;
using Interfaces;

namespace Command_Pattern
{
    public class CommandInvoker
    {
        public async Task ExecuteCommand(List<ICommand> commands)
        {
            foreach (var command in commands)
            {
                await command.Execute();
            }
        }
    }
}