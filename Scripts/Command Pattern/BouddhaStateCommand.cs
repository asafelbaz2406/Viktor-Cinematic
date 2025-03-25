using System.Threading.Tasks;
using Interfaces;
using UnityEngine;

namespace Command_Pattern
{
    public class BouddhaStateCommand : ViktorCommand
    {
        public BouddhaStateCommand(IEntity viktor) : base(viktor) { }  
        
        public override async Task Execute()
        {
            Viktor.BouddhaState();
            await Awaitable.WaitForSecondsAsync(Viktor.Animations.BuddhaState());
            Viktor.Animations.BuddhaState();
        }
    }
}