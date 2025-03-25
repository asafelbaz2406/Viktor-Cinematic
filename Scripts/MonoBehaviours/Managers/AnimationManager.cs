using System.Collections.Generic;
using SO;
using UnityEngine;

namespace MonoBehaviours.Managers
{
    public class AnimationManager : MonoBehaviour
    {
        private Animator _animator;

        [SerializeField] private AnimationSettingsData viktorAnimationSettingsData;

        private static readonly int BuddhaHash = Animator.StringToHash("Ult_intro_Anim");
        private static readonly int LightningStrikeHash = Animator.StringToHash("Recall_Anim");   
        private static readonly int LaserHash = Animator.StringToHash("Laser_Idle_Anim"); 
        private static readonly int SummonClonesHash = Animator.StringToHash("Idle_Anim");
        
        private const float CrossFadeDuration = 0.2f;
        private int _currentAnimationIndex; 
        
        private readonly Dictionary<int, float> _animationDuration = new()
        {
            { LaserHash, 0.2f },
            { SummonClonesHash, 0.2f },
            { BuddhaHash, 0.2f },
            { LightningStrikeHash, 0.2f}
        };
        
        private void Awake() => _animator = GetComponent<Animator>();
    
        public float Laser() => PlayAnimation(LaserHash, viktorAnimationSettingsData.waitTimeLaserBeam);
        public float SummonClones() => PlayAnimation(BuddhaHash, viktorAnimationSettingsData.waitTimeSummonClones);
        public float BuddhaState() => PlayAnimation(BuddhaHash,viktorAnimationSettingsData.waitTimeBuddhaState); 
        public float LightningStrike() => PlayAnimation(LightningStrikeHash, viktorAnimationSettingsData.waitTimeLightningStrike);
        public float EarthAttack() => PlayAnimation(BuddhaHash, viktorAnimationSettingsData.waitTimeEarthAttack);
        public float ScreenMelt() => PlayAnimation(BuddhaHash, viktorAnimationSettingsData.waitTimeScreenMeltAfterLaser);

        private float PlayAnimation(int animationHash, float waitTimeBeforeNextAnimation)  
        {
            if (_currentAnimationIndex == animationHash &&
                 _currentAnimationIndex == BuddhaHash) 
            {
                 // Stay in Buddha state
                 return _animationDuration[animationHash] + waitTimeBeforeNextAnimation;
            }
             
            _currentAnimationIndex = animationHash;
            _animator.CrossFade(animationHash, CrossFadeDuration);
            return _animationDuration[animationHash] + waitTimeBeforeNextAnimation;
        }
    }
}