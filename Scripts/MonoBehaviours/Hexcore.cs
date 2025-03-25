using Interfaces;
using MonoBehaviours.Managers;
using SO;
using Unity.Cinemachine;
using UnityEngine;

namespace MonoBehaviours
{
    public class Hexcore : MonoBehaviour, IClickable
    {
        [SerializeField] private CinemachineImpulseSource impulseSource;
        [SerializeField] private ScreenShakeProfile screenShakeProfile;
        public void OnClick()
        {
            //Debug.Log("OnClick", gameObject);
            CameraShakeManager.Instance.ScreenShakeFromProfile(screenShakeProfile, impulseSource);
        }
    }
}
