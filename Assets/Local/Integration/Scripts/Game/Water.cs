using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using UnityEngine.UI; 

namespace Local.Integration.Scripts.Game
{
    public class Water : MonoBehaviour
    {
        [SerializeField] private Transform _waterPlane;
        [SerializeField] private float _drowningTime = 5f; 
        [SerializeField] private float _vignetteMaxIntensity = 1f; 
        [SerializeField] private Image _blackScreen; 
        [SerializeField] private Volume _postProcessVolume;

        private bool _isInWater = false;
        private float _timeInWater = 0f;

        private Vignette _vignette;

        private void Start()
        {
            if (_postProcessVolume.profile.TryGet(out _vignette))
            {
                _vignette.intensity.value = 0f;
                _vignette.smoothness.value = 0.5f;
                _vignette.center.value = new Vector2(0.5f, 0.5f); 
                _vignette.rounded.value = false;
            }

            if (_blackScreen != null)
            {
                _blackScreen.color = new Color(0, 0, 0, 0); 
            }
        }

        private void Update()
        {
            if (_isInWater)
            {
                _timeInWater += Time.deltaTime;

                if (_vignette != null)
                {
                    float progress = _timeInWater / _drowningTime;

                    _vignette.intensity.value = Mathf.Lerp(0f, _vignetteMaxIntensity, progress);
                    _vignette.smoothness.value = Mathf.Lerp(0.5f, 1f, progress);
                }

                if (_blackScreen != null)
                {
                    float alpha = Mathf.SmoothStep(0f, 1f, _timeInWater / _drowningTime);
                    _blackScreen.color = new Color(0, 0, 0, alpha);
                }

                if (_timeInWater >= _drowningTime)
                {
                    HandleDrowning();
                }
            }
            else if (_timeInWater > 0)
            {
                _timeInWater -= Time.deltaTime;

                if (_vignette != null)
                {
                    float progress = _timeInWater / _drowningTime;
                    _vignette.intensity.value = Mathf.Lerp(0f, _vignetteMaxIntensity, progress);
                    _vignette.smoothness.value = Mathf.Lerp(0.5f, 1f, progress);
                }

                if (_blackScreen != null)
                {
                    float alpha = Mathf.SmoothStep(0f, 1f, _timeInWater / _drowningTime);
                    _blackScreen.color = new Color(0, 0, 0, alpha);
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
            _timeInWater = _drowningTime; 
            _vignette.intensity.value = _vignetteMaxIntensity;
            _vignette.smoothness.value = 1f;
            _blackScreen.color = new Color(0, 0, 0, 1);
            GameManager.instance.Win();
        }
    }
}
