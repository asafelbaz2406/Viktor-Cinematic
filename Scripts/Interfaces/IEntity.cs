using MonoBehaviours.Managers;

namespace Interfaces
{
    public interface IEntity
    {
        public void Laser();
        public void SummonClones();
        public void BouddhaState();
        public void LightningStrike();
        public void EarthAttack();
        
        AnimationManager Animations { get; }
    }
}