using UnityEngine;

namespace MonoBehaviours.AnimationsScripts
{
    public class LaserBeamLine : MonoBehaviour
    {
        [SerializeField] private LineRenderer[] lineRenderers;
        [SerializeField] private Transform beamCoreStartTransform;
        [SerializeField] private Transform beamCoreEndTransform;
        [SerializeField] private Transform beamBodyTransform;
        [SerializeField] private Transform particleSystemBeamCore;
        
        private Vector3[] _positions = new Vector3[2];
        private const float BeamLengthAmplifier = 0.5f;
        private const float BeamCoreYOffset = -0.05f;

        public void SetOriginPositionAndTargetPosition(Vector3 originPosition, Vector3 targetPosition, float laserBeamLength, Vector3 viktorForwardDirection)
        {
            if (lineRenderers == null || lineRenderers.Length == 0 || beamCoreStartTransform == null || beamBodyTransform == null || beamCoreEndTransform == null) return;
            
            _positions[0] = originPosition;
            _positions[1] = targetPosition;

            foreach (var lineRenderer in lineRenderers)
            {
                if (lineRenderer != null)
                {
                    lineRenderer.SetPositions(_positions);
                }
            }

            beamCoreStartTransform.position = originPosition + new Vector3(0f, BeamCoreYOffset, 0f);
            particleSystemBeamCore.position = beamCoreStartTransform.position;
            beamCoreEndTransform.position = targetPosition + new Vector3(0f, BeamCoreYOffset, 0f);
            
            SetBeamBody(originPosition, laserBeamLength, viktorForwardDirection);
        }

        // The beam core is a cube, and its pivot point has been changed through blender,
        // and we scale its size by the y-axis, so we assign the up direction to be viktor's forward direction
        private void SetBeamBody(Vector3 originPosition, float laserBeamLength, Vector3 viktorForwardDirection)
        {
            beamBodyTransform.transform.up = viktorForwardDirection;
            beamBodyTransform.position = originPosition;
            beamBodyTransform.localScale = new Vector3(beamBodyTransform.localScale.x, BeamLengthAmplifier * laserBeamLength, beamBodyTransform.localScale.z);
        }
    }
}
