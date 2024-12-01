using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Local.Noah.Scripts.MENU
{
    public class AudioManager : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider soundsSlider;
        [SerializeField] private Button toggleMusicButton;
        [SerializeField] private Button toggleSoundButton;
        [SerializeField] private AudioMixer _audioMixer;

        [Header("Audio Sources")]
        public AudioSource musicSource;
        public AudioSource soundSource;

        private bool isMusicEnabled;
        private bool isSoundEnabled;

        void Start()
        {
            musicSlider.value = musicSource.volume;
            soundsSlider.value = soundSource.volume;

            isMusicEnabled = true;
            isSoundEnabled = true;
        }

        public void SetMusicVolume(float volume)
        {
            if (isMusicEnabled)
            {
                _audioMixer.SetFloat("musicVolume", Mathf.Log10(volume) * 20f);
                musicSource.volume = volume; 
            }
        }

        public void SetSoundVolume(float volume)
        {
            if (isSoundEnabled)
            {
                _audioMixer.SetFloat("soundsVolume", Mathf.Log10(volume) * 20f);
                soundSource.volume = volume;
            }
        }

        public void ToggleMusic()
        {
            if (isMusicEnabled)
            {
                musicSlider.interactable = false; 
                musicSource.volume = 0; 
                musicSlider.value = 0; 
                toggleMusicButton.GetComponentInChildren<TextMeshProUGUI>().text = "OFF";
                isMusicEnabled = false; 
            }
            else
            {
                musicSlider.interactable = true; 
                musicSource.volume = musicSlider.value; 
                toggleMusicButton.GetComponentInChildren<TextMeshProUGUI>().text = "ON";
                isMusicEnabled = true;
            }
        }

        public void ToggleSound()
        {
            if (isSoundEnabled) 
            {
                soundsSlider.interactable = false; 
                soundSource.volume = 0; 
                soundsSlider.value = 0; 
                toggleSoundButton.GetComponentInChildren<TextMeshProUGUI>().text = "OFF"; 
                isSoundEnabled = false; 
            }
            else
            {
                soundsSlider.interactable = true; 
                soundSource.volume = soundsSlider.value; 
                toggleSoundButton.GetComponentInChildren<TextMeshProUGUI>().text = "ON"; 
                isSoundEnabled = true; 
            }
        }
    }
}
