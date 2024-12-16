using System;
using System.Collections.Generic;
using DG.Tweening;
using Local.Integration.Scripts.SCORE;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Local.Integration.Scripts.Game
{
    public class GameManager : MonoBehaviour
    {
        public ScoreData ScoreData;
        public static GameManager instance;
        public int GameplayTime;
        public int CountdownTime;
        public string StartText;
        public int EndTime;
        public string EndText;
        public bool HasStarted;
        public bool HasEnded;

        public float RemainingTime;

        [SerializeField] private GameObject _bungalowUIGo;
        [SerializeField] private GameObject _panelWin;
        [SerializeField] private GameObject _panelGameOver;
        [SerializeField] private Canvas _gameCanvas;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _collectedItemsText;
        [SerializeField] private TextMeshPro _floatingTextPrefab;
        private Dictionary<string, int> _validatedItems = new Dictionary<string, int>();

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
            CloseBungalowCanvas();
            LoadBestScore();
            UpdateScoreUI();
        }

        private void Start()
        {
            RemainingTime = GameplayTime + CountdownTime;
        }
        
        private void Update()
        {
            RemainingTime -= Time.deltaTime;
        }

        public void UpdateScoreUI()
        {
            if (_scoreText != null)
            {
                _scoreText.text = $"Score Total : {ScoreData.CurrentScore} points";
            }
        }

        public void AddScore(int points)
        {
            ScoreData.CurrentScore += points;

            if (ScoreData.CurrentScore > ScoreData.BestScore)
            {
                ScoreData.BestScore = ScoreData.CurrentScore;
                SaveBestScore();
            }

            UpdateScoreUI();
        }

        public void ResetScore()
        {
            ScoreData.CurrentScore = 0;
            _validatedItems.Clear();
            UpdateScoreUI();
            UpdateCollectedItemsUI();
        }

        private void LoadBestScore()
        {
            ScoreData.BestScore = PlayerPrefs.GetInt("BestScore", 0);
        }

        private void SaveBestScore()
        {
            PlayerPrefs.SetInt("BestScore", ScoreData.BestScore);
            PlayerPrefs.Save();
        }

        private void OpenBungalowCanvas()
        {
            _bungalowUIGo.gameObject.SetActive(true);
        }

        public void CloseBungalowCanvas()
        {
            _bungalowUIGo.gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                OpenBungalowCanvas();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                CloseBungalowCanvas();
            }
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

                AddScore(item.Score * item.Quantity);
            }

            UpdateCollectedItemsUI();

            OpenBungalowCanvas();
        }

        public void Win()
        {
            HasEnded = true;
            _gameCanvas.enabled = false;
            _panelWin.transform.localScale = Vector3.zero;
            _panelWin.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);            
        }

        public void GameOver()
        {
            HasEnded = true;
            _gameCanvas.enabled = false;
            _panelGameOver.transform.localScale = Vector3.zero;
            _panelGameOver.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
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
            floatingText.transform.rotation = Quaternion.Euler(0, 180, 0); 
            floatingText.DOFade(0, 1f).SetEase(Ease.InCubic).OnComplete(() => Destroy(floatingText.gameObject));
            Destroy(floatingText, destroyTime);
        }

    }
}
