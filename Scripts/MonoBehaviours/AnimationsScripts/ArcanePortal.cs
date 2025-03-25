using DG.Tweening;
using UnityEngine;
using UnityEngine.VFX;

namespace MonoBehaviours.AnimationsScripts
{
    [RequireComponent(typeof(VisualEffect))]
    public class ArcanePortal : MonoBehaviour
    {
        [SerializeField] private VisualEffect vfx;
        [SerializeField] private float alphaTweeningDuration = 2f;
        [SerializeField] private Ease easeType = Ease.InQuart;
        
        private Tween _alphaTween; // Store tween reference

        #region Alpha Settings
        private float _startAlpha;
        private readonly int _portalAlpha = Shader.PropertyToID("PortalAlpha");
        private float _currentAlpha;
        private const float TargetAlphaIn = 1f;
        private const float TargetAlphaOut = 0f;
        #endregion

        private void OnEnable()
        {
            FadeIn();
        }
        
        private void OnDisable()
        {
            StopActiveTween();
        }

        private void Fade(float startValue, float endValue)
        {
            // Stop any existing fade before starting a new one
            StopActiveTween();
            
            // Reset the alpha
            _startAlpha = startValue;
            
            // Set initial alpha
            vfx.SetFloat(_portalAlpha, _startAlpha);
            
            // Tween alpha and apply the updated value to the VFX Graph
            _alphaTween = DOTween.To(() => _startAlpha, x =>
                {
                    _startAlpha = x;
                    vfx.SetFloat(_portalAlpha, _startAlpha);
                }, endValue, alphaTweeningDuration)
                .SetEase(easeType)
                .SetId(vfx); // Assign vfx as the ID
        }
        private void StopActiveTween()
        {
            if (_alphaTween != null && _alphaTween.IsActive())
            {
                _alphaTween.Kill(); // Kill the active tween before starting a new one
            }
        }

        private void SetAlphaBeforeFade() =>  _currentAlpha = vfx.GetFloat(_portalAlpha);
        
        public void FadeOut()
        {
            SetAlphaBeforeFade();
            if (Mathf.Approximately(_currentAlpha, TargetAlphaIn)) return; 
            
            Fade(_currentAlpha, TargetAlphaOut);
        }

        private void FadeIn()
        {
            SetAlphaBeforeFade();
            if (Mathf.Approximately(_currentAlpha, TargetAlphaIn)) return; 
            
            Fade(_currentAlpha, TargetAlphaIn);
        }
    }
}
