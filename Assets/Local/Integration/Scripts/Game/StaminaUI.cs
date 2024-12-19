using UnityEngine;

namespace Local.Integration.Scripts.Game
{
    public class StaminaUI : MonoBehaviour
    {
        [SerializeField] private Transform _playerTransform;
        [SerializeField] private Vector3 _offset;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private float _smoothTime = 0.1f;

        private RectTransform _rectTransform;
        private Vector2 _velocity = Vector2.zero;

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
                    out Vector2 targetPosition
                );

                _rectTransform.localPosition = Vector2.SmoothDamp(_rectTransform.localPosition, targetPosition, ref _velocity, _smoothTime);
            }
        }
    }
}