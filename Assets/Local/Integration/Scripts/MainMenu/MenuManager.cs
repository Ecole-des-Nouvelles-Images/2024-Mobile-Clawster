using UnityEngine;
using UnityEngine.EventSystems;

namespace Local.Integration.Scripts.MainMenu
{
    public class MenuManager : MonoBehaviour
    {
        [Header("Menu Objects")]
        [SerializeField] private GameObject _mainMenuCanvasGo;
        [SerializeField] private GameObject _settingsMenuCanvasGo;
        [SerializeField] private GameObject _volumeMenuCanvasGo;
        [SerializeField] private GameObject _creditsMenuCanvasGo;



        [Header("First Selected Options")] [SerializeField]
        private GameObject _mainMenuFirst;

        [SerializeField] private GameObject _settingsMenuFirst;
        [SerializeField] private GameObject _volumeMenuFirst;
        
        private void Start()
        {
            _mainMenuCanvasGo.SetActive(true);
            _settingsMenuCanvasGo.SetActive(false);
            _volumeMenuCanvasGo.SetActive(false);
            _creditsMenuCanvasGo.SetActive(false);

        }
        
        #region Canvas Activations

        private void OpenMainMenu()
        {
            _mainMenuCanvasGo.SetActive(true);
            _settingsMenuCanvasGo.SetActive(false);
            _volumeMenuCanvasGo.SetActive(false);
            _creditsMenuCanvasGo.SetActive(false);

            EventSystem.current.SetSelectedGameObject(_mainMenuFirst);
        }

        private void OpenSettingsMenuHandle()
        {
            _mainMenuCanvasGo.SetActive(false);
            _settingsMenuCanvasGo.SetActive(true);
            _volumeMenuCanvasGo.SetActive(false);
            _creditsMenuCanvasGo.SetActive(false);
        }

        private void OpenVolumeMenuHandle()
        {
            _mainMenuCanvasGo.SetActive(false);
            _settingsMenuCanvasGo.SetActive(false);
            _volumeMenuCanvasGo.SetActive(true);
            _creditsMenuCanvasGo.SetActive(false);
        }
        
        
        private void OpenCreditsMenuHandle()
        {
            _mainMenuCanvasGo.SetActive(false);
            _settingsMenuCanvasGo.SetActive(false);
            _volumeMenuCanvasGo.SetActive(false);
            _creditsMenuCanvasGo.SetActive(true);
        }


        private void CloseAllMenus()
        {
            _mainMenuCanvasGo.SetActive(false);
            _settingsMenuCanvasGo.SetActive(false);
            _volumeMenuCanvasGo.SetActive(false);
            _creditsMenuCanvasGo.SetActive(false);
        }

        #endregion

        #region Main Menu Button Actions

        public void OnSettingsPress()
        {
            OpenSettingsMenuHandle();
        }
        #endregion

        #region Settings Menu Button Actions

        public void OnSettingsVolumePress()
        {
            OpenVolumeMenuHandle();
        }
        
        public void OnSettingsCreditsPress()
        {
            OpenCreditsMenuHandle();
        }

        public void OnSettingsBackPress()
        {
            OpenMainMenu();
        }

        #endregion
    }
}