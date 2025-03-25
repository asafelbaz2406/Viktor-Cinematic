using System.Threading.Tasks;
using Interfaces;
using UnityEngine;

namespace Command_Pattern
{
    public class LightningStrikeCommand : ViktorCommand
    {
        public LightningStrikeCommand(IEntity viktor) : base(viktor) { } 
        
        public override async Task Execute()
        {
            Viktor.LightningStrike();
            await Awaitable.WaitForSecondsAsync(Viktor.Animations.LightningStrike());
            Viktor.Animations.LightningStrike();
        }
    }
}