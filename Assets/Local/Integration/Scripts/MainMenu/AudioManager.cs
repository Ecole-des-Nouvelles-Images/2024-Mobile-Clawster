using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;
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
            _musicSlider.value = MusicSource.volume;
            _soundsSlider.value = SoundSource.volume;

            _isMusicEnabled = true;
            _isSoundEnabled = true;
        }

        public void SetMusicVolume(float volume)
        {
            if (_isMusicEnabled)
            {
                _audioMixer.SetFloat("musicVolume", Mathf.Log10(volume) * 20f);
                MusicSource.volume = volume; 
            }
        }

        public void SetSoundVolume(float volume)
        {
            if (_isSoundEnabled)
            {
                _audioMixer.SetFloat("soundsVolume", Mathf.Log10(volume) * 20f);
                SoundSource.volume = volume;
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
                MusicSource.volume = _musicSlider.value; 
                _toggleMusicButton.GetComponentInChildren<TextMeshProUGUI>().text = "ON";
                _isMusicEnabled = true;
            }
        }

        public void ToggleSound()
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
                SoundSource.volume = _soundsSlider.value; 
                _toggleSoundButton.GetComponentInChildren<TextMeshProUGUI>().text = "ON"; 
                _isSoundEnabled = true; 
            }
        }
    }
}
