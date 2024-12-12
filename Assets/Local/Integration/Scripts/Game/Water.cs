using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI; 

namespace Local.Integration.Scripts.Game
{
    public class Water : MonoBehaviour
    {
        [SerializeField] private Transform _waterPlane;
        [SerializeField] private float drowningTime = 5f; 
        [SerializeField] private float vignetteMaxIntensity = 1f; 
        [SerializeField] private Image blackScreen; 

        private bool _isInWater = false;
        private float _timeInWater = 0f;

        private Vignette _vignette;
        private Volume _postProcessVolume;

        private void Start()
        {
            _postProcessVolume = FindObjectOfType<Volume>();
            if (_postProcessVolume.profile.TryGet(out _vignette))
            {
                _vignette.intensity.value = 0f;
                _vignette.smoothness.value = 0.5f;
                _vignette.center.value = new Vector2(0.5f, 0.5f); 
                _vignette.rounded.value = false;
            }

            if (blackScreen != null)
            {
                blackScreen.color = new Color(0, 0, 0, 0); 
            }
        }

        private void Update()
        {
            if (_isInWater)
            {
                _timeInWater += Time.deltaTime;

                if (_vignette != null)
                {
                    float progress = _timeInWater / drowningTime;

                    _vignette.intensity.value = Mathf.Lerp(0f, vignetteMaxIntensity, progress);
                    _vignette.smoothness.value = Mathf.Lerp(0.5f, 1f, progress);
                }

                if (blackScreen != null)
                {
                    float alpha = Mathf.SmoothStep(0f, 1f, _timeInWater / drowningTime);
                    blackScreen.color = new Color(0, 0, 0, alpha);
                }

                if (_timeInWater >= drowningTime)
                {
                    HandleDrowning();
                }
            }
            else if (_timeInWater > 0)
            {
                _timeInWater -= Time.deltaTime;

                if (_vignette != null)
                {
                    float progress = _timeInWater / drowningTime;
                    _vignette.intensity.value = Mathf.Lerp(0f, vignetteMaxIntensity, progress);
                    _vignette.smoothness.value = Mathf.Lerp(0.5f, 1f, progress);
                }

                if (blackScreen != null)
                {
                    float alpha = Mathf.SmoothStep(0f, 1f, _timeInWater / drowningTime);
                    blackScreen.color = new Color(0, 0, 0, alpha);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("HeadPlayer") && !_isInWater)
            {
                _isInWater = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("HeadPlayer") && _isInWater)
            {
                _isInWater = false;
            }
        }

        private void HandleDrowning()
        {
            _timeInWater = drowningTime; 
            _vignette.intensity.value = vignetteMaxIntensity;
            _vignette.smoothness.value = 1f;
            blackScreen.color = new Color(0, 0, 0, 1);
        }
    }
}
