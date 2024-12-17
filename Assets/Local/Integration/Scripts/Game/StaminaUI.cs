using UnityEngine;

namespace Local.Integration.Scripts.Game
{
    public class StaminaUI : MonoBehaviour
    {
        [SerializeField] private Transform _playerTransform; 
        [SerializeField] private Vector3 _offset; 
        [SerializeField] private Canvas _canvas; 

        private RectTransform _rectTransform;

        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        private void LateUpdate()
        {
            if (_playerTransform != null && _canvas != null)
            {
                Vector3 screenPosition = Camera.main.WorldToScreenPoint(_playerTransform.position + _offset);

                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _canvas.GetComponent<RectTransform>(), 
                    screenPosition, 
                    _canvas.worldCamera, 
                    out Vector2 localPosition
                );

                _rectTransform.localPosition = localPosition;
            }
        }
    }
}