using System;
using DG.Tweening;
using Interfaces;
using MonoBehaviours.AnimationsScripts;
using MonoBehaviours.Managers;
using Sirenix.Utilities;
using SO;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.VFX;

namespace MonoBehaviours.Viktor
{
    [RequireComponent(typeof(Material), typeof(Animator))]
    public class Viktor : MonoBehaviour, IEntity
    {
        #region Settings
        [Header("Animated Objects")]
        [Header("Animation Settings")]
        [SerializeField] private Animator animator;
        [SerializeField] private AnimationSettingsData viktorAnimationSettingsData;
        
        [Header("Laser Beam Settings")]
        [SerializeField] private LaserBeamLine laserBeam;
        [SerializeField] private Transform laserBeamSpawnPosition;
        [SerializeField] private float laserBeamDuration;
        [SerializeField] private float laserBeamSize;
        [SerializeField] private float mainViktorBeamSizeAmplifier = 1f;
        
        [Header("Camera Shake")]
        [SerializeField] private ScreenShakeProfile lightningStrikeProfile;
        [SerializeField] private CinemachineImpulseSource impulseSource;
        [SerializeField] private PlayableDirector playableDirector;

        [Header("Clones Settings")]
        [SerializeField] private bool isClone;
        [SerializeField] private GameObject cloneParticleSystem;
        
        [Header("Lightning Strike Settings")]
        [SerializeField] private GameObject vfxLightningStrike;
        [SerializeField] private Transform lightningStrikeSpawnPosition; 
        
        [Header("Earth Attack Settings")]
        [SerializeField] private VisualEffect[] vfxEarthAttacks;
        #endregion
        
        private AnimationManager _animationManager; 
        public GameObject GetCloneParticleSystem() => IsClone() ? cloneParticleSystem : null;
        public AnimationManager Animations => _animationManager ??= GetComponent<AnimationManager>();
        public Action OnLaserBeamMeltFullScreen;
        public bool IsClone() => isClone;
        
        public void LaserBeam()
        {
            // Cast Laser
            animator.CrossFade(viktorAnimationSettingsData.LaserIdle, viktorAnimationSettingsData.transitionToLaserBeamTime);
        }

        // This function is getting called from an animation event in the Laser_Beam_Anim animation - after 10 frames
        public void CastLaserAnimationEvent()
        {
            //ScreenMeltAfterMainLaser();
            OnLaserBeamMeltFullScreen?.Invoke();
            
            var laserBeamLen = IsClone() ? laserBeamSize : laserBeamSize * mainViktorBeamSizeAmplifier;
            var laserBeamOriginPosition = laserBeamSpawnPosition.position;
            var laserBeamTargetPosition = laserBeamOriginPosition + transform.forward * laserBeamLen;
            
            SetLaserBeamSettingsBeforeCasting(laserBeamOriginPosition, laserBeamTargetPosition, laserBeamLen, transform.forward);
        }

        private void SetLaserBeamSettingsBeforeCasting(Vector3 laserSpawnPosition, Vector3 laserTargetPosition, float laserBeamLength, Vector3 viktorTransformForward)
        {
            if (laserBeam == null || laserBeam.isActiveAndEnabled) return;
            
            Debug.DrawRay(start: transform.position, dir: transform.forward * laserBeamLength * 2,Color.red, duration: 20f);
            
            laserBeam.transform.position = Vector3.zero;
            laserBeam.transform.rotation = Quaternion.identity;
            
            laserBeam.SetOriginPositionAndTargetPosition(laserSpawnPosition, laserTargetPosition, laserBeamLength, viktorTransformForward);
            laserBeam.gameObject.SetActive(true);
        }

        // This function is getting called at the end of the Laser animation
        private void EndLaserAnimationEvent()
        {
            if (laserBeam != null && laserBeam.isActiveAndEnabled)
            {
                laserBeam.gameObject.SetActive(false);
            }
        }
        
        // This function is getting called from an animation event in the Recall animation - after 8 seconds
        public void RecallAnimationEventLightningStrike()
        {
            if (vfxLightningStrike == null) return;
            
            var spawnPosition = new Vector3(lightningStrikeSpawnPosition.position.x, 0f, lightningStrikeSpawnPosition.position.z);
            vfxLightningStrike.transform.position = spawnPosition;
            
            const float delayBeforeCameraShake = 0.02f;
            const float lightningActiveTime = 3f;
            
            var sequence = DOTween.Sequence();
            
            sequence
                .AppendCallback(() => vfxLightningStrike.SetActive(true))
                .AppendInterval(delayBeforeCameraShake)
                .AppendCallback(() => CameraShakeManager.Instance.ScreenShakeFromProfile(lightningStrikeProfile, impulseSource))
                .AppendInterval(lightningActiveTime)
                .AppendCallback(() => vfxLightningStrike.SetActive(false));
        }
        
        public void Laser()
        {
            Debug.Log("Laser");
        }

        public void SummonClones()
        {
            Debug.Log("Summon Clones");

            ClonesManager.Instance.ClonesLaser();
        }

        public void BouddhaState()
        {
            Debug.Log("Buddha State");
        }
        
        public void LightningStrike()
        {
            Debug.Log("Lightning Strike");
            
            playableDirector.Play();
        }

        public void EarthAttack()
        {
            Debug.Log("Earth Attack");
            if (vfxEarthAttacks.IsNullOrEmpty() || !vfxEarthAttacks[0] || !vfxEarthAttacks[1])
            {
                Debug.Log("VFX EARTH ATTACK ERROR");
                return;
            }

            const float delayBeforeSummoningTime = 1.8f;
            const float activeTime = 1.5f;
            const float freezeVfxTime = 1.5f;
            const float fadeTime = 3.5f;
            
            var sequence = DOTween.Sequence();
            sequence
                .AppendInterval(delayBeforeSummoningTime)
                .AppendCallback(() => ToggleVFX(isActive: true, vfxEarthAttacks))
                .AppendInterval(activeTime)
                .AppendCallback(() => TogglePauseVFX(isPaused: true, vfxEarthAttacks))
                .AppendInterval(freezeVfxTime)
                .AppendCallback(() => TogglePauseVFX(isPaused: false, vfxEarthAttacks))
                .AppendInterval(fadeTime)
                .AppendCallback(() => ToggleVFX(isActive: false, vfxEarthAttacks));
        }
        
        private static void ToggleVFX(bool isActive, VisualEffect[] vfxObjects) 
        {
            foreach (var vfx in vfxObjects)
            {
                if (vfx) vfx.gameObject.SetActive(isActive);
            }
        }

        private static void TogglePauseVFX(bool isPaused, VisualEffect[] vfxObjects) 
        {
            foreach (var vfx in vfxObjects)
            {
                if (vfx) vfx.pause = isPaused;
            }
        }
    }
}
