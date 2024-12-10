using System;
using UnityEngine;
using Cinemachine;
using UnityEngine.Serialization;

namespace Local.Integration.Scripts.Game
{
    public class CameraZoneSwitcher : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _playZoneCamera;
        [SerializeField] private CinemachineVirtualCamera _safeZoneCamera;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _safeZoneCamera.enabled = true;
                _playZoneCamera.enabled = false;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _playZoneCamera.enabled = true;
                _safeZoneCamera.enabled = false;
            }
        }
    }
}