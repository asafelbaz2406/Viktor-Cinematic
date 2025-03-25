using UnityEngine;

namespace SO
{
    [CreateAssetMenu(menuName = "AnimationsData/DecalAnimationData")]
    public class DecalAnimationData : ScriptableObject
    {
        [Header("Scale")]
        public Vector3 decalSphereStartTargetScale;
        public Vector3 decalSphereEndTargetScale;
        
        public readonly int ShapeTexIntensity = Shader.PropertyToID("_ShapeTexIntensity");
        
        [Header("Shape Texture Intensity Settings")]
        public float shapeTextureStartValue;
        public float shapeTextureEndValue;
        public float shapeTextureDuration;
        public float particleShutDownShrinkDelay;
        public float rotationSpeed;
    }
}
