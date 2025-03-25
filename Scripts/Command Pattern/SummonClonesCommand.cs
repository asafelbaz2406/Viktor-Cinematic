using System.Threading.Tasks;
using Interfaces;
using UnityEngine;

namespace Command_Pattern
{
    public class SummonClonesCommand : ViktorCommand
    {
        public SummonClonesCommand(IEntity viktor) : base(viktor) { }  
        
        public override async Task Execute()
        {
            Viktor.SummonClones();
            await Awaitable.WaitForSecondsAsync(Viktor.Animations.SummonClones());
            Viktor.Animations.SummonClones();
        }
    }
}