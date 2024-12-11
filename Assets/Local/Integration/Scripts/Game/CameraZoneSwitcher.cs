using UnityEngine;
using Cinemachine;

namespace Local.Integration.Scripts.Game
{
    public class CameraZoneSwitcher : MonoBehaviour
    {
        [SerializeField] private string triggerTag;
        
        [SerializeField] private CinemachineVirtualCamera _primaryCamera;
        [SerializeField] private CinemachineVirtualCamera[] _virtualCameras;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(triggerTag))
            {
                CinemachineVirtualCamera targetCamera = other.GetComponentInChildren<CinemachineVirtualCamera>();
                SwitchToCamera(targetCamera);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(triggerTag))
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