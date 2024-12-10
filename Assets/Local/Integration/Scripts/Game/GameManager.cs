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
        public int CountdownTime;       //Time before game starts
        public string StartText;        //Go! or Bin It!
        public int EndTime;             //Time before game ends
        public string EndText;          //Finish! or Stop!

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