using UnityEngine;
using UnityEngine.VFX;

namespace MonoBehaviours.AnimationsScripts
{
    public class LaserBeam : MonoBehaviour
    {
        [SerializeField] private VisualEffect[] vfxGraph;
        private float _laserBeamSize = 5f;
        private float _laserBeamDuration = 5f;
        private Vector3 _lookAtPos = Vector3.zero;
        private readonly int _duration = Shader.PropertyToID("Duration");
        private readonly int _size = Shader.PropertyToID("Size"); 
        private readonly int _lookAtPosition = Shader.PropertyToID("LookAtPosition");

        public void SetBeamSizeAndDuration(float laserBeamSize, float laserBeamDuration, Vector3 lookAtPosition) 
        {
            _laserBeamSize = laserBeamSize;
            _laserBeamDuration = laserBeamDuration;
            _lookAtPos = lookAtPosition;

            foreach (var vfxGraphChild in vfxGraph)
            {
                if (vfxGraphChild == null) continue;
                
                if (vfxGraphChild.HasFloat(_duration))
                {
                    vfxGraphChild.SetFloat(_duration, _laserBeamDuration);
                }
                    
                if (vfxGraphChild.HasFloat(_size))
                {
                    vfxGraphChild.SetFloat(_size, _laserBeamSize);
                }
                
                if (vfxGraphChild.HasFloat(_lookAtPosition))
                {
                    vfxGraphChild.SetVector3(_lookAtPosition, _lookAtPos);
                }
                
            }
        }
    }
}
