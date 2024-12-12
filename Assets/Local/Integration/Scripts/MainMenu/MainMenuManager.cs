using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Local.Integration.Scripts.MainMenu
{
    public class MainMenuManager : MonoBehaviour
    {
        [Header("Main Menu Objects")]
        [SerializeField] private GameObject _loadingBarObject;
        [SerializeField] private Image _loadingBar;
        [SerializeField] private GameObject[] _objectsToHide;

        [Header("Scene to Load")]
        [SerializeField] private string _gameScene = "Game" ;

        private List<AsyncOperation> _scenesToLoad = new List<AsyncOperation>();

        private void Awake()
        {
            _loadingBarObject.SetActive(false);
        }

        public void StartGame()
        {
            Debug.Log("StartGame called");

            HideMenu();
            _loadingBarObject.SetActive(true);
            AsyncOperation sceneLoadOperation = SceneManager.LoadSceneAsync(_gameScene);
            _scenesToLoad.Add(sceneLoadOperation);
            StartCoroutine(ProgressLoadingBar());
        }


        private void HideMenu()
        {
            for (int i = 0; i < _objectsToHide.Length; i++)
            {
                _objectsToHide[i].SetActive(false);
            }
        }

        private IEnumerator ProgressLoadingBar()
        {
            float loadProgress = 0f;
            for (int i = 0; i < _scenesToLoad.Count; i++)
            {
                while (!_scenesToLoad[i].isDone)
                {
                    loadProgress = Mathf.Lerp(loadProgress, _scenesToLoad[i].progress, Time.deltaTime);
                    _loadingBar.fillAmount = loadProgress;
                    yield return null;
                }
            }
        }
    }
}