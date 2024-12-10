using Local.Integration.Scripts.SCORE;
using UnityEngine;
using UnityEngine.Serialization;

namespace Local.Integration.Scripts.Game
{
    public class GameManager : MonoBehaviour
    {
        public ScoreData ScoreData;
        public static GameManager instance;
        
        [Header("Game Start / End Variables")]
        public int CountdownTime;
        public string StartText;
        public int EndTime;
        public string EndText;

        public bool HasStarted;

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
    }
}