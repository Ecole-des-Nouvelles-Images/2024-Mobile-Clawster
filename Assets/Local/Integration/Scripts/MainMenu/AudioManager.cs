using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Local.Integration.Scripts.MainMenu
{
    public class AudioManager : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Slider _musicSlider;
        [SerializeField] private Slider _soundsSlider;
        [SerializeField] private Button _toggleMusicButton;
        [SerializeField] private Button _toggleSoundButton;
        [SerializeField] private AudioMixer _audioMixer;

        [Header("Audio Sources")]
        public AudioSource MusicSource;
        public AudioSource SoundSource;

        private bool _isMusicEnabled;
        private bool _isSoundEnabled;

        private const string MusicVolumeKey = "MusicVolume";
        private const string SoundsVolumeKey = "SoundsVolume";
        private const string MusicEnabledKey = "MusicEnabled";
        private const string SoundsEnabledKey = "SoundsEnabled";

        void Start()
        {
            LoadSettings();
        }

        public void SetMusicVolume(float sliderValue)
        {
            if (_isMusicEnabled)
            {
                float linearVolume = sliderValue / 8f;
                float decibelVolume = MapToDecibelRange(linearVolume, -80f, 0f);
                _audioMixer.SetFloat("musicVolume", decibelVolume);
                MusicSource.volume = linearVolume;
                PlayerPrefs.SetFloat(MusicVolumeKey, sliderValue);
            }
        }

        public void SetSoundsVolume(float sliderValue)
        {
            if (_isSoundEnabled)
            {
                float linearVolume = sliderValue / 8f;
                float decibelVolume = MapToDecibelRange(linearVolume, -80f, 0f);
                _audioMixer.SetFloat("soundsVolume", decibelVolume);
                SoundSource.volume = linearVolume;
                PlayerPrefs.SetFloat(SoundsVolumeKey, sliderValue);
            }
        }

        public void ToggleMusic()
        {
            _isMusicEnabled = !_isMusicEnabled;
            UpdateMusicToggleButton();
            PlayerPrefs.SetInt(MusicEnabledKey, _isMusicEnabled ? 1 : 0);
        }

        public void ToggleSounds()
        {
            _isSoundEnabled = !_isSoundEnabled;
            UpdateSoundToggleButton();
            PlayerPrefs.SetInt(SoundsEnabledKey, _isSoundEnabled ? 1 : 0);
        }

        private float MapToDecibelRange(float linear, float minDb, float maxDb)
        {
            return Mathf.Lerp(minDb, maxDb, linear);
        }

        private void LoadSettings()
        {
            _musicSlider.value = PlayerPrefs.GetFloat(MusicVolumeKey, 8f);
            _soundsSlider.value = PlayerPrefs.GetFloat(SoundsVolumeKey, 8f);

            _isMusicEnabled = PlayerPrefs.GetInt(MusicEnabledKey, 1) == 1;
            _isSoundEnabled = PlayerPrefs.GetInt(SoundsEnabledKey, 1) == 1;

            SetMusicVolume(_musicSlider.value);
            SetSoundsVolume(_soundsSlider.value);

            UpdateMusicToggleButton();
            UpdateSoundToggleButton();
        }

        private void UpdateMusicToggleButton()
        {
            _musicSlider.interactable = _isMusicEnabled;
            _toggleMusicButton.GetComponentInChildren<TextMeshProUGUI>().text = _isMusicEnabled ? "ON" : "OFF";
            if (!_isMusicEnabled)
            {
                MusicSource.volume = 0;
                _musicSlider.value = 0;
            }
            else
            {
                MusicSource.volume = _musicSlider.value / 8f;
            }
        }

        private void UpdateSoundToggleButton()
        {
            _soundsSlider.interactable = _isSoundEnabled;
            _toggleSoundButton.GetComponentInChildren<TextMeshProUGUI>().text = _isSoundEnabled ? "ON" : "OFF";
            if (!_isSoundEnabled)
            {
                SoundSource.volume = 0;
                _soundsSlider.value = 0;
            }
            else
            {
                SoundSource.volume = _soundsSlider.value / 8f;
            }
        }
    }
}
