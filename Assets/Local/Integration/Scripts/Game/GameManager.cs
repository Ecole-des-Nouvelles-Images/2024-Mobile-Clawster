using System.Collections.Generic;
using DG.Tweening;
using Local.Integration.Scripts.MainMenu;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Local.Integration.Scripts.Game

{
    public class GameManager : MonoBehaviour
    {
        [Header("Game Settings")] 
        public static GameManager instance;
        public ScoreData ScoreData;
        public float GameTime = 120f;
        public float ElapsedTime;

        public int CountdownTime;
        public string StartText;
        public int EndTime;
        public string EndText;
        public bool HasStarted;
        public bool HasEnded;

        [Header("UI Elements")] 
        [SerializeField] private Image _timerFillImage;
        [SerializeField] private GameObject _bungalowUI;
        [SerializeField] private GameObject _blackScreen;
        [SerializeField] private GameObject _panelWin;
        [SerializeField] private TextMeshProUGUI _panelWinText;
        [SerializeField] private GameObject _panelGameOver;
        [SerializeField] private Canvas _gameCanvas;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _collectedItemsText;
        [SerializeField] private TextMeshPro _floatingTextPrefab;
        private Dictionary<string, int> _validatedItems = new Dictionary<string, int>();

        [Header("Sound Effects")] 
        [SerializeField] private AudioSource _gameMusic;
        [SerializeField] private AudioSource _gameAtmosphere;
        [SerializeField] private AudioClip _victorySE;
        [SerializeField] private AudioClip _defeatSE;


        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            ResetScore();
            UpdateScoreUI();
        }
        
        private void Update()
        {
            if (ElapsedTime < GameTime)
            {
                ElapsedTime += Time.deltaTime;
                _timerFillImage.fillAmount = Mathf.Clamp01(1 - (ElapsedTime / GameTime));
            }
            else
            {
                //Win();
            }
        }

        public void UpdateScoreUI()
        {
            if (_scoreText != null)
            {
                _scoreText.text = $"Score Total : {ScoreData.CurrentScore} points";
            }
        }
        
        private void ResetScore()
        {
            ScoreData.CurrentScore = 0;
            _validatedItems.Clear();
            UpdateScoreUI();
            UpdateCollectedItemsUI();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                OpenBangalowUI();
            }
        }
        

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                CloseBangalowUI();
            }
        }
        
        private void OpenBangalowUI()
        {
            _bungalowUI.SetActive(true);
        }

        public void CloseBangalowUI()
        {
            _bungalowUI.SetActive(false);
        }
        
        public void DisplayCollectedItems(Dictionary<string, CollectedItemData> collectedItems)
        {
            foreach (var item in collectedItems.Values)
            {
                if (_validatedItems.ContainsKey(item.Name))
                {
                    _validatedItems[item.Name] += item.Quantity;
                }
                else
                {
                    _validatedItems[item.Name] = item.Quantity;
                }

                ScoreManager.instance.AddScore(item.Score * item.Quantity);
            }

            UpdateCollectedItemsUI();
        }

        public void Win()
        {
            if (!HasEnded)
            {
                HasEnded = true;
                if (_gameMusic != null && _gameMusic.isPlaying)
                {
                    _gameMusic.Stop();
                }
                UpdateWinUI();
                _gameCanvas.enabled = false;
                _panelWin.transform.localScale = Vector3.zero;
                _panelWin.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
                SoundFXManager.instance.PlaySoundFXClip(_victorySE, transform, 1f);
            }
        }
        
        public void GameOver()
        {
            if (!HasEnded)
            {
                HasEnded = true;
                if (_gameMusic != null && _gameMusic.isPlaying)
                {
                    _gameMusic.Stop();
                }
                _blackScreen.SetActive(true);
                _gameCanvas.enabled = false;
                _panelGameOver.transform.localScale = Vector3.zero;
                _panelGameOver.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);

                SoundFXManager.instance.PlaySoundFXClip(_defeatSE, transform, 1f);
            }
        }


        private void UpdateWinUI()
        {
            _panelWinText.text = "Ton Score\n " + $"{ScoreData.CurrentScore}\n" + "\n" + "Meilleur Score\n " +
                                 $"{ScoreData.BestScore}";
        }

        private void UpdateCollectedItemsUI()
        {
            _collectedItemsText.text = "Objets validés :\n";

            foreach (var item in _validatedItems)
            {
                _collectedItemsText.text += $"{item.Key} x{item.Value}\n";
            }
        }

        public void ShowFloatingText(Vector3 spawnPosition, float destroyTime)
        {
            TextMeshPro floatingText = Instantiate(_floatingTextPrefab, spawnPosition, Quaternion.identity, transform);
            floatingText.transform.position = spawnPosition + new Vector3(0, 0, -2);
            floatingText.transform.rotation = Quaternion.Euler(0, 180, 0);
            floatingText.DOFade(0, 1f).SetEase(Ease.InCubic).OnComplete(() => Destroy(floatingText.gameObject));
            Destroy(floatingText, destroyTime);
        }
        
        public void PlayAgain()
        {
            ResetScore();
            ElapsedTime = 0f;
            HasStarted = true;
            HasEnded = false;

            _gameCanvas.enabled = true;
            _panelWin.SetActive(false);
            _panelGameOver.SetActive(false);
            _blackScreen.SetActive(false);

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
        
        public void GoToMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }

    }
}