using UnityEngine;
using UnityEngine.Serialization;

namespace Local.Noah.Scripts.GAME
{
    public class ScoreManager : MonoBehaviour
    {
        public ScoreData ScoreData;

        private static ScoreManager Instance { get; set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
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

        public void LoadBestScore()
        {
            ScoreData.BestScore = PlayerPrefs.GetInt("BestScore", 0);
        }

        public void SaveBestScore()
        {
            PlayerPrefs.SetInt("BestScore", ScoreData.BestScore);
            PlayerPrefs.Save();
        }
    }
}