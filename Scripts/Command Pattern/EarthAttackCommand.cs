using System.Threading.Tasks;
using Interfaces;
using UnityEngine;

namespace Command_Pattern
{
    public class EarthAttackCommand : ViktorCommand
    {
        public EarthAttackCommand(IEntity viktor) : base(viktor) { } 
        
        public override async Task Execute()
        {
            Viktor.EarthAttack();
            await Awaitable.WaitForSecondsAsync(Viktor.Animations.EarthAttack());
            Viktor.Animations.EarthAttack();
        }
    }
}