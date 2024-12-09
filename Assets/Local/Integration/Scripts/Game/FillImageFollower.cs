using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class FillImageFollower : MonoBehaviour
{
    [SerializeField] private Image _fillImage; 
    [SerializeField] private RectTransform _followerImage; 

    void Update()
    {
        if (_fillImage != null && _followerImage != null)
        {
            RectTransform fillRect = _fillImage.GetComponent<RectTransform>();
            float fillWidth = fillRect.rect.width;
            float positionX = fillRect.rect.xMin + (fillWidth * _fillImage.fillAmount);
            _followerImage.localPosition = new Vector3(positionX, _followerImage.localPosition.y, _followerImage.localPosition.z);
        }
    }
}