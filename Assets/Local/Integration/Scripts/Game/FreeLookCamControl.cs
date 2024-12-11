using System;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.EventSystems;

namespace Local.Integration.Scripts.Game
{
    public class FreeLookCamControl : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        private Image _imageCamControlArea;
        [SerializeField] private CinemachineFreeLook _camFreeLook;
        private string _strMouseX = "Mouse X";
        private string _strMouseY = "Mouse Y";

        private void Start()
        {
            _imageCamControlArea = GetComponent<Image>();

            if (_camFreeLook == null)
            {
                Debug.LogError("CinemachineFreeLook component is not assigned!");
            }

            if (_imageCamControlArea == null)
            {
                Debug.LogError("Image component for camera control is missing!");
            }
        }


        public void OnDrag(PointerEventData eventData)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_imageCamControlArea.rectTransform,
                    eventData.position, eventData.enterEventCamera, out Vector2 posOut))
            {
                _camFreeLook.m_XAxis.m_InputAxisName = _strMouseX;
                _camFreeLook.m_YAxis.m_InputAxisName = _strMouseY;

            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _camFreeLook.m_XAxis.m_InputAxisName = null;
            _camFreeLook.m_YAxis.m_InputAxisName = null;
            _camFreeLook.m_XAxis.m_InputAxisValue = 0;
            _camFreeLook.m_YAxis.m_InputAxisValue = 0;
        }
    }
}
