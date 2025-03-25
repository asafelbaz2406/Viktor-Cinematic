using DG.Tweening;
using SO;
using UnityEngine;
using UnityEditor;

namespace MonoBehaviours.AnimationsScripts
{
    [ExecuteInEditMode]
    public class Decal : MonoBehaviour
    {
        [SerializeField] private Material mMaterial;
     
        [Header("PropertyBlock Settings")]
        [SerializeField] private Texture2D texture;
        [SerializeField] private string textureName = "_MainTex";
        
        [ColorUsage(true, true)]
        [SerializeField] private Color color = Color.white;
        [SerializeField] private string colorName = "_Tint";
     
        // mesh to draw with
        private Mesh _mCubeMesh;
        // extra settings
        private MaterialPropertyBlock _props;
        // render settings
        private RenderParams _renderParams;

        [Header("Decal Spawn Settings")] 
        [SerializeField] private DecalAnimationData decalAnimationData;
        [SerializeField] private bool startAnimation = true;
        [SerializeField] private new ParticleSystem particleSystem;
        public float FullySizedDecalTime { get; set; }
        
        private void SetPropertyBlockSettings() 
        {
            _props ??= new MaterialPropertyBlock();
            if (texture)
            {
                _props.SetTexture(textureName, texture);
            }
            _props.SetColor(colorName, color);
     
        }
     
        private void OnValidate()
        {
            SetPropertyBlockSettings();
        }
        
        public void OnEnable()
        {
            SetPropertyBlockSettings();
            _mCubeMesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            _renderParams = new RenderParams(mMaterial)
            {
                matProps = _props, 
                receiveShadows = false
            };
            
            if (startAnimation)
            {
                RecallAnimationSpawn();
            }
        }
     
        #if UNITY_EDITOR
            private void DrawGizmo(bool selected)
            {
                // draw a cube dizmo and a line in the direction of the decal
                var col = new Color(0.0f, 0.7f, 1f, 1.0f);
                col.a = selected ? 0.3f : 0.1f;
                Gizmos.color = col;
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(Vector3.zero, Vector3.one);
                col.a = selected ? 0.5f : 0.05f;
                Gizmos.color = col;
                Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
                Handles.matrix = transform.localToWorldMatrix;
                Handles.DrawBezier(Vector3.zero, Vector3.down, Vector3.zero, Vector3.down, Color.red, null, selected ? 4f : 2f);
            }
        #endif
     
     
        #if UNITY_EDITOR
            public void OnDrawGizmos()
            {
                DrawGizmo(false);
            }
            public void OnDrawGizmosSelected()
            {
                DrawGizmo(true);
            }
        #endif
        
        private void Update() 
        {
            // draw the cube using the render parameters
            Graphics.RenderMesh(_renderParams, _mCubeMesh, 0, transform.localToWorldMatrix);
            
            // Rotating the decal object 
            //transform.Rotate(0f, Time.deltaTime * decalAnimationData.rotationSpeed, 0f);
        }

        public float GetShapeTextureDurationTime() => decalAnimationData.shapeTextureDuration;
        public float GetTotalTime() => FullySizedDecalTime + decalAnimationData.shapeTextureDuration;
        
        private void RecallAnimationSpawn()
        {
            // Resetting the size
            transform.localScale = Vector3.zero;
            
            // Scaling the decal object and tweening the _ShapeTexIntensity property in the shader
            // stopping the particle system before shrinking back to end target scale and tweening the _ShapeTexIntensity property back to 0 
            
            var sequence = DOTween.Sequence();
            
            var scaleAndTweenShapeTexIntensityToMax = DOTween.Sequence();
            scaleAndTweenShapeTexIntensityToMax.Append(transform.DOScale(decalAnimationData.decalSphereEndTargetScale,
                    decalAnimationData.shapeTextureDuration))
                    .Join(DOTween.To(() => decalAnimationData.shapeTextureStartValue,
                    x => mMaterial.SetFloat(decalAnimationData.ShapeTexIntensity, x),
                    decalAnimationData.shapeTextureEndValue, decalAnimationData.shapeTextureDuration));

            var shrinkAndTweenShapeTexIntensity = DOTween.Sequence();
            shrinkAndTweenShapeTexIntensity.Append(transform.DOScale(decalAnimationData.decalSphereStartTargetScale,
                    decalAnimationData.shapeTextureDuration))
                    .Join(DOTween.To(() => decalAnimationData.shapeTextureEndValue,
                    x => mMaterial.SetFloat(decalAnimationData.ShapeTexIntensity, x),
                    decalAnimationData.shapeTextureStartValue, decalAnimationData.shapeTextureDuration)).SetEase(Ease.OutSine)
                    .AppendCallback(TurnOffParticleSystem);
            
            
            var turnOffParticlesTime = FullySizedDecalTime - decalAnimationData.particleShutDownShrinkDelay;
            
            sequence.Append(scaleAndTweenShapeTexIntensityToMax)
                    .AppendInterval(turnOffParticlesTime)
                    .AppendCallback(TurnOffParticleSystem)
                    .AppendInterval(decalAnimationData.particleShutDownShrinkDelay) 
                    .Append(shrinkAndTweenShapeTexIntensity)
                    .OnComplete(() => gameObject.SetActive(false));
        }
        
        // Function to stop the Particle System
        private void TurnOffParticleSystem()
        {
            if (particleSystem == null) return;
            
            particleSystem.Stop(); // Stop emission
            particleSystem.Clear(); // Clear existing particles
        }
    }
}
