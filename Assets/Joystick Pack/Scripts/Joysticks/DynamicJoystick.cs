using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Joystick_Pack.Scripts.Joysticks
{
    public class DynamicJoystick : Joystick
    {
        public float MoveThreshold { get { return _moveThreshold; } set { _moveThreshold = Mathf.Abs(value); } }
        private Vector2 _startPosition;
        [SerializeField] private float _moveThreshold = 1;

        protected override void Start()
        {
            MoveThreshold = _moveThreshold;
            base.Start();
            _startPosition = background.anchoredPosition;
            // background.gameObject.SetActive(false);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
            // background.gameObject.SetActive(true);
            base.OnPointerDown(eventData);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            // background.gameObject.SetActive(false);
            base.OnPointerUp(eventData);
            background.anchoredPosition = _startPosition;
        }

        protected override void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera cam)
        {
            if (magnitude > _moveThreshold)
            {
                Vector2 difference = normalised * (magnitude - _moveThreshold) * radius;
                background.anchoredPosition += difference;
            }
            base.HandleInput(magnitude, normalised, radius, cam);
        }
    }
}