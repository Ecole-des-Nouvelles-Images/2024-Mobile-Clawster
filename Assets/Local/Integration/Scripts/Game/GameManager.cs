using System;
using Local.Integration.Scripts.SCORE;
using UnityEngine;
using UnityEngine.Serialization;

namespace Local.Integration.Scripts.Game
{
    public class GameManager : MonoBehaviour
    {
        public ScoreData ScoreData;
        public static GameManager instance;
        public int CountdownTime;
        public string StartText;
        public int EndTime;
        public string EndText;
        public bool HasStarted;

       [SerializeField] private GameObject _bungalowUIGo;
       
        
        private int _holdScore;
        private int _holdWeight;

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
            CloseBungalowCanvas();
            LoadBestScore();
        }
        
        public void AddScore(int points)
        {
            ScoreData.CurrentScore += points;
            if (ScoreData.CurrentScore > ScoreData.BestScore)
            {
                ScoreData.BestScore = ScoreData.CurrentScore;
                SaveBestScore();
            }
        }

        public void ResetScore()
        {
            ScoreData.CurrentScore = 0;
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
            if (other.CompareTag("Player")) ;
            {
                OpenBungalowCanvas();
            }
        }
    }
}