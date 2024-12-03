using UnityEngine;
using UnityEngine.EventSystems;

namespace Joystick_Pack.Scripts.Joysticks
{
    public class DynamicJoystick : Joystick
    {
        public float MoveThreshold { get { return moveThreshold; } set { moveThreshold = Mathf.Abs(value); } }
        private Vector2 _startPosition;
        [SerializeField] private float moveThreshold = 1;

        protected override void Start()
        {
            MoveThreshold = moveThreshold;
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
            if (magnitude > moveThreshold)
            {
                Vector2 difference = normalised * (magnitude - moveThreshold) * radius;
                background.anchoredPosition += difference;
            }
            base.HandleInput(magnitude, normalised, radius, cam);
        }
    }
}