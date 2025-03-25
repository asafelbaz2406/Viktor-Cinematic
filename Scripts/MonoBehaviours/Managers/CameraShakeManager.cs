using SO;
using Unity.Cinemachine;
using UnityEngine;

namespace MonoBehaviours.Managers
{
    public class CameraShakeManager : MonoBehaviour
    {
        public static CameraShakeManager Instance { get; private set;}

        [SerializeField] private float globalShakeForce = 1f;
        [SerializeField] private CinemachineImpulseListener impulseListener;

        private CinemachineImpulseDefinition _impulseDefinition;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void CameraShake(CinemachineImpulseSource impulseSource)
        {
            impulseSource.GenerateImpulseWithForce(globalShakeForce);
        }

        public void ScreenShakeFromProfile(ScreenShakeProfile profile, CinemachineImpulseSource impulseSource)
        {
            // apply settings
            SetupScreenShakeSettings(profile, impulseSource);
            
            //screenshake
            impulseSource.GenerateImpulseWithForce(profile.impulseForce);
        }

        private void SetupScreenShakeSettings(ScreenShakeProfile profile, CinemachineImpulseSource impulseSource)
        {
            _impulseDefinition = impulseSource.ImpulseDefinition;

            // change the impulse source settings
            _impulseDefinition.ImpulseDuration = profile.impulseTime;
            impulseSource.DefaultVelocity = profile.defaultVelocity;
            _impulseDefinition.CustomImpulseShape = profile.impulseCurve;
            
            // change the impulse listener settings
            impulseListener.ReactionSettings.AmplitudeGain = profile.listenerAmplitude;
            impulseListener.ReactionSettings.FrequencyGain = profile.listenerFrequency;
            impulseListener.ReactionSettings.Duration = profile.listenerDuration;
        }
    }
}
