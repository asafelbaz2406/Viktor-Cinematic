using System.Collections.Generic;
using System.Threading.Tasks;
using Command_Pattern;
using Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MonoBehaviours.Managers
{
    public class CommandManager : SerializedMonoBehaviour
    {
        [SerializeField] private IEntity _entity;
        [SerializeField] private ICommand _singleCommand;
        [SerializeField] private List<ICommand> _commands;
        
        private readonly CommandInvoker _commandInvoker = new();
        private bool _isCommandExecuting; 

        private void Start()
        {
            _entity = GetComponent<IEntity>();

            _singleCommand = ViktorCommand.Create<EarthAttackCommand>(_entity);

            _commands = new List<ICommand>
            {
                ViktorCommand.Create<LightningStrikeCommand>(_entity),
                ViktorCommand.Create<EarthAttackCommand>(_entity),
                ViktorCommand.Create<SummonClonesCommand>(_entity),
                ViktorCommand.Create<LaserCommand>(_entity),
            };
        }

        private async Task ExecuteCommands(List<ICommand> commands)
        {
            _isCommandExecuting = true;
            await _commandInvoker.ExecuteCommand(commands);
            _isCommandExecuting = false;
        }

        private void Update()
        {
            if (_isCommandExecuting) return;
            
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ExecuteCommands(new List<ICommand> {_singleCommand });
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ExecuteCommands(_commands);
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                ExecuteCommands(new List<ICommand> { new LaserCommand(_entity) });
            }
        }
    }
}