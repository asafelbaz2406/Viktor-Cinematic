using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using SO;
using UnityEngine;
using ViktorClass = MonoBehaviours.Viktor.Viktor;


namespace MonoBehaviours.Managers
{
    public class ClonesManager : MonoBehaviour
    {
        public static ClonesManager Instance { get; private set; } 
        
        [Header("Scriptable Objects")]
        [SerializeField] private ErosionSettingsData erosionSettingsData;
        [SerializeField] private CinematicSettingsData cinematicSettingsData;
        [SerializeField] private DecalAnimationData blackHoleAnimationData;
        [SerializeField] private AnimationSettingsData animationSettingsData;
        
        [Header("Cinematic Settings")]
        [SerializeField] private ViktorClass viktorMain;
        
        [Header("Clones Settings")]
        [SerializeField] private GameObject clone;
        [SerializeField] private int clonesCount;
        [SerializeField] private Transform[] clonesSpawnPosition;
        [SerializeField] private Material cloneViktorMaterial;
        
        private readonly List<GameObject> _summonedClones = new List<GameObject>(3);
        private readonly List<ViktorClass> _viktors = new List<ViktorClass>(3); 
        private readonly List<GameObject> _clonesParticleSystems = new List<GameObject>(3);

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
        
        private void Start()
        {
            // Reset the clone shader everytime I press start
            cloneViktorMaterial.SetFloat(erosionSettingsData.RevealValue, erosionSettingsData.minErodeValue);
            
            for (var i = 0; i < clonesCount; i++)
            {
                var summonedClone = Instantiate(clone, clonesSpawnPosition[i].position, clonesSpawnPosition[i].rotation, viktorMain.transform);
                
                _summonedClones.Add(summonedClone);
                _viktors.Add(summonedClone.GetComponent<Viktor.Viktor>());
                _clonesParticleSystems.Add(_viktors[i].GetCloneParticleSystem());
                
                summonedClone.gameObject.SetActive(false);
            }
        }
        
        // This is getting called from the Viktor script, inside the chain animation
        // Viktor.SummonClones()
        public void ClonesLaser()
        {
            var delayAfterSummoning = erosionSettingsData.particleSystemDelay + erosionSettingsData.erodingTime;
            var delayAfterFadeInAndLaser = erosionSettingsData.erodingTime + erosionSettingsData.waitBetweenLaserAndErosion;
            var totalLaserTime = erosionSettingsData.laserBeamDuration + erosionSettingsData.waitBetweenLaserAndErosion;
            
            var sequenceErodeIn = DOTween.Sequence();
            sequenceErodeIn
                .AppendCallback(() => ActivateClone(true))
                .AppendInterval(delayAfterSummoning)
                .AppendCallback(() => ActivateParticleSystem(true))
                .AppendCallback(FadeInClones)
                .AppendInterval(delayAfterFadeInAndLaser)
                .AppendCallback(ClonesLaserBeam)
                .AppendInterval(totalLaserTime)
                .AppendCallback(() => ActivateParticleSystem(false))
                .AppendCallback(FadeOutClones);
        }
        
        private void ActivateClone(bool activate) => _summonedClones.ForEach(summonedClone => summonedClone.SetActive(activate));
        private void ActivateParticleSystem(bool activate) => _clonesParticleSystems.ForEach(cloneParticleSystem => cloneParticleSystem.SetActive(activate));
        
        private void ClonesLaserBeam()
        {
            foreach (var viktor in _viktors.Where(viktor => viktor.IsClone()))
            {
                viktor.LaserBeam();
            }
        }
        
        private void Erode(float target, float startValue, TweenCallback onComplete = null)
        {
            var erodeTime = erosionSettingsData.erodingTime;
            
            DOTween.To(() => startValue,
                    x => cloneViktorMaterial.SetFloat(erosionSettingsData.RevealValue, x),
                    target,
                    erodeTime)
                .SetEase(Ease.InCubic)
                .OnComplete(onComplete);
        }
        
        private void FadeInClones() => Erode(target: erosionSettingsData.maxErodeValue, startValue: erosionSettingsData.minErodeValue);
        private void FadeOutClones() => Erode(target: erosionSettingsData.minErodeValue, startValue: erosionSettingsData.maxErodeValue, onComplete: () => ActivateClone(false));
    }
}
