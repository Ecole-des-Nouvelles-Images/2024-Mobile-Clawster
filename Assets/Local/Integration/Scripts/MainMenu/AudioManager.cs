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

        void Start()
        {
            _musicSlider.value = MusicSource.volume * 100f;
            _soundsSlider.value = SoundSource.volume * 100f;

            _isMusicEnabled = true;
            _isSoundEnabled = true;
        }

        public void SetMusicVolume(float sliderValue)
        {
            if (_isMusicEnabled)
            {
                float linearVolume = sliderValue / 8f;
                float decibelVolume = MapToDecibelRange(linearVolume, -40f, 0f);
                _audioMixer.SetFloat("musicVolume", decibelVolume);
                MusicSource.volume = linearVolume;
            }
        }

        public void SetSoundsVolume(float sliderValue)
        {
            if (_isSoundEnabled)
            {
                float linearVolume = sliderValue / 8f;
                float decibelVolume = MapToDecibelRange(linearVolume, -40f, 0f);
                _audioMixer.SetFloat("soundsVolume", decibelVolume);
                SoundSource.volume = linearVolume;
            }
        }

        public void ToggleMusic()
        {
            if (_isMusicEnabled)
            {
                _musicSlider.interactable = false;
                MusicSource.volume = 0;
                _musicSlider.value = 0;
                _toggleMusicButton.GetComponentInChildren<TextMeshProUGUI>().text = "OFF";
                _isMusicEnabled = false;
            }
            else
            {
                _musicSlider.interactable = true;
                MusicSource.volume = _musicSlider.value / 8f;
                _toggleMusicButton.GetComponentInChildren<TextMeshProUGUI>().text = "ON";
                _isMusicEnabled = true;
            }
        }

        public void ToggleSounds()
        {
            if (_isSoundEnabled)
            {
                _soundsSlider.interactable = false;
                SoundSource.volume = 0;
                _soundsSlider.value = 0;
                _toggleSoundButton.GetComponentInChildren<TextMeshProUGUI>().text = "OFF";
                _isSoundEnabled = false;
            }
            else
            {
                _soundsSlider.interactable = true;
                SoundSource.volume = _soundsSlider.value / 8f;
                _toggleSoundButton.GetComponentInChildren<TextMeshProUGUI>().text = "ON";
                _isSoundEnabled = true;
            }
        }

        private float MapToDecibelRange(float linear, float minDb, float maxDb)
        {
            return Mathf.Lerp(minDb, maxDb, linear);
        }
    }
}
