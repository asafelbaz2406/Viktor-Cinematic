using DG.Tweening;
using UnityEngine;

namespace MonoBehaviours.AnimationsScripts
{
    public class FulLScreenEffect : MonoBehaviour
    {
        [SerializeField] private Viktor.Viktor mainViktor;
    
        [Header("Screen Melt Settings")]
        [SerializeField] private Material fullScreenMeltMaterial;
        [SerializeField] private float fullScreenMeltStartValue = 15f;
        private static readonly int VignettePower = Shader.PropertyToID("_VignettePower");
        
        private void Start()
        {
            if(fullScreenMeltMaterial)
                fullScreenMeltMaterial.SetFloat(VignettePower, fullScreenMeltStartValue);
        }

        #region Subscribe and Unsubscribe Methods
        private void OnEnable()
        {
            mainViktor.OnLaserBeamMeltFullScreen += ScreenMeltAfterMainLaser;
        }

        private void OnDisable()
        {
            mainViktor.OnLaserBeamMeltFullScreen -= ScreenMeltAfterMainLaser;
        }
        #endregion

        private void ScreenMeltAfterMainLaser()
        {
            if (!mainViktor || mainViktor.IsClone() || !fullScreenMeltMaterial) return;
            
            const float endValue = 4f;
            const float duration = 2.5f;
            DOTween.To(
                getter: () => fullScreenMeltMaterial.GetFloat(VignettePower),
                setter: (value) => fullScreenMeltMaterial.SetFloat(VignettePower, value),
                endValue: endValue,
                duration: duration
            );
        }
    }
}
