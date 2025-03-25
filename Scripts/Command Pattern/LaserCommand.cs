using System.Threading.Tasks;
using Interfaces;
using UnityEngine;

namespace Command_Pattern
{
    public class LaserCommand : ViktorCommand
    {
        public LaserCommand(IEntity viktor) : base(viktor) { }
        
        public override async Task Execute()
        {
            Viktor.Laser();
            await Awaitable.WaitForSecondsAsync(Viktor.Animations.Laser());
            Viktor.Animations.Laser();
        }
    }
}