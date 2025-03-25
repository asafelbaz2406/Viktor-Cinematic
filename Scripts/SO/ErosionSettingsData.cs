using UnityEngine;

namespace SO
{
    [CreateAssetMenu(menuName = "ErosionSettingsData/ErosionSettingsData")]
    public class ErosionSettingsData : ScriptableObject
    {
        public float erodingTime = 2f; 
        public float currentRevealValue = 1f;
        public float timeElapsed;
        public bool canErode; 
        public float minErodeValue;
        public float maxErodeValue;
        public float waitBetweenLaserAndErosion;
        public float laserBeamDuration;
        public float particleSystemDelay;
        public readonly int RevealValue = Shader.PropertyToID("_RevealValue");
    }
}
