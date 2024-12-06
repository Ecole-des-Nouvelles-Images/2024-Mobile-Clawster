using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Local.Noah.Scripts.MENU
{
    public class MenuManager : MonoBehaviour
    {
        [Header("Menu Objects")] [SerializeField]
        private GameObject _mainMenuCanvasGo;

        [SerializeField] private GameObject _settingsMenuCanvasGo;
        [SerializeField] private GameObject _volumeMenuCanvasGo;


        [Header("First Selected Options")] [SerializeField]
        private GameObject _mainMenuFirst;

        [SerializeField] private GameObject _settingsMenuFirst;
        [SerializeField] private GameObject _volumeMenuFirst;

        [SerializeField] private string _levelScene;


        private bool _isPaused;

        private void Start()
        {
            _mainMenuCanvasGo.SetActive(false);
            _settingsMenuCanvasGo.SetActive(false);
            _volumeMenuCanvasGo.SetActive(false);
        }

        private void Update()
        {
            if (!_isPaused)
            {
                Pause();
            }
            else
            {
                Unpause();
            }
        }

        #region Pause/Unpause Functions

        private void Pause()
        {
            _isPaused = true;
            Time.timeScale = 0f;
            OpenMainMenu();
        }

        private void Unpause()
        {
            _isPaused = false;
            Time.timeScale = 1f;
            CloseAllMenus();
        }

        #endregion

        #region Canvas Activations

        private void OpenMainMenu()
        {
            _mainMenuCanvasGo.SetActive(true);
            _settingsMenuCanvasGo.SetActive(false);
            _volumeMenuCanvasGo.SetActive(false);

            EventSystem.current.SetSelectedGameObject(_mainMenuFirst);
        }

        private void OpenSettingsMenuHandle()
        {
            _settingsMenuCanvasGo.SetActive(true);
            _mainMenuCanvasGo.SetActive(false);
            _volumeMenuCanvasGo.SetActive(false);
        }

        private void OpenVolumeMenuHandle()
        {
            _volumeMenuCanvasGo.SetActive(true);
            _mainMenuCanvasGo.SetActive(false);
            _settingsMenuCanvasGo.SetActive(false);
        }


        private void CloseAllMenus()
        {
            _mainMenuCanvasGo.SetActive(false);
            _settingsMenuCanvasGo.SetActive(false);
            _volumeMenuCanvasGo.SetActive(false);
        }

        #endregion

        #region Main Menu Button Actions

        public void OnSettingsPress()
        {
            OpenSettingsMenuHandle();
        }

        public void OnResumePress()
        {
            Unpause();
        }

        public void OnBackToMenuQuit()
        {
            SceneManager.LoadSceneAsync(_levelScene);
        }

        #endregion

        #region Settings Menu Button Actions

        public void OnSettingsVolumePress()
        {
            OpenVolumeMenuHandle();
        }

        public void OnSettingsBackPress()
        {
            OpenMainMenu();
        }

        #endregion
    }
}