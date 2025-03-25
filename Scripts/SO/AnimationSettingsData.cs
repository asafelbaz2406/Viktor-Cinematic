using UnityEngine;

namespace SO
{
    [CreateAssetMenu(menuName = "AnimationsData/ViktorAnimationsData")]
    public class AnimationSettingsData : ScriptableObject
    {
        public bool isUlting; 
        public float transitionFromRunToUltTime;
        public float transitionFromUltToRunTime = 0.2f; 
        public float transitionToRecallTime = 0.2f;
        public float fullySizedDecalTime = 5f;
        public float transitionToLaserBeamTime = 0.2f;
        public float transitionToIdle = 0.2f;
        public readonly int UltIntroAnimation = Animator.StringToHash("Ult_intro_Anim");
        public readonly int RunBaseAnim = Animator.StringToHash("Run_Base_Anim");  
        public readonly int RecallAnim = Animator.StringToHash("Recall_Anim");  
        public readonly int LaserBeamAnim = Animator.StringToHash("Laser_Beam_Anim");
        public readonly int LaserIdle = Animator.StringToHash("Laser_Idle_Anim"); 
        public readonly int IdleAnim = Animator.StringToHash("Idle_Anim");
        
        [Header("Wait Time For Animation")]
        public float waitTimeLaserBeam;
        public float waitTimeSummonClones;
        public float waitTimeLightningStrike;
        public float waitTimeBuddhaState;
        public float waitTimeEarthAttack;
        public float waitTimeScreenMeltAfterLaser;
    }
}
