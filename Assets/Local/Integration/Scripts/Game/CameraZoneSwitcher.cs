using UnityEngine;
using Cinemachine;
using UnityEngine.Serialization;

namespace Local.Integration.Scripts.Game
{
    public class CameraZoneSwitcher : MonoBehaviour
    {
        [SerializeField] private string _triggerTag;
        [SerializeField] private CinemachineVirtualCamera _primaryCamera;
        [SerializeField] private CinemachineVirtualCamera[] _virtualCameras;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(_triggerTag))
            {
                CinemachineVirtualCamera targetCamera = other.GetComponentInChildren<CinemachineVirtualCamera>();
                SwitchToCamera(targetCamera);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(_triggerTag))
            {
                SwitchToCamera(_primaryCamera);
            }
        }

        private void SwitchToCamera(CinemachineVirtualCamera targetCamera)
        {
            foreach (CinemachineVirtualCamera camera in _virtualCameras)
            {
                camera.enabled = camera == targetCamera;
            }
        }
    }
}