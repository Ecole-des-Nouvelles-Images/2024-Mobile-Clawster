using UnityEngine;

namespace Local.Integration.Scripts.Game
{
    public class StaminaUI : MonoBehaviour
    {
        [SerializeField] private Transform _playerTransform;
        [SerializeField] private Vector3 _offset;
        [SerializeField] private Camera _mainCamera;

        private RectTransform rectTransform;

        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
            }
        }

        private void LateUpdate()
        {
            if (_playerTransform != null && _mainCamera != null)
            {
                rectTransform.position = _playerTransform.position + _offset;
            }
        }
    }
}
