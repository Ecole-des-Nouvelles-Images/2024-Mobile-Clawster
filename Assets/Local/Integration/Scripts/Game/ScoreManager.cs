using System.Collections.Generic;
using DG.Tweening;
using Local.Integration.Scripts.MainMenu;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Local.Integration.Scripts.Game
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager instance;
        public ScoreData ScoreData;
        
        public int HoldScore;
        
        [SerializeField] private ClawsterController _clawsterController;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _panelWinText;
        
        private Dictionary<string, int> _validatedItems = new Dictionary<string, int>();
        private Dictionary<string, CollectedItemData> _collectedItems = new Dictionary<string, CollectedItemData>();


        
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
            LoadBestScore();
            UpdateScoreUI();
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
        
        private void ResetScore()
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

        private void UpdateScoreUI()
        {
            if (_scoreText != null)
            {
                _scoreText.text = $"Score Total : {ScoreData.CurrentScore} points";
            }
        }
        
        private void UpdateScoreWinUI()
        {
            _panelWinText.text = "Ton Score\n " + $"{ScoreData.CurrentScore}\n" + "\n" + "Meilleur Score\n " +
                                 $"{ScoreData.BestScore}";
        }
        
        public void ValidateScore()
        { 
            AddScore(HoldScore);
            GameManager.instance.DisplayCollectedItems(_collectedItems);
            HoldScore = 0;
            _clawsterController._weightHold = 0;
            _collectedItems.Clear();
            // _weightFillImage.DOFillAmount(0, 0.2f).SetEase(Ease.InOutQuad);
            GameManager.instance.UpdateScoreUI();
            // SoundFXManager.instance.PlaySoundFXClip(_validateSE, transform, 1f);
        }
    }
}
