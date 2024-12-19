using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Local.Integration.Scripts.Game
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager instance;

        [Header("Joystick")]
        [SerializeField] private Joystick _joystick;
        
        [Header("Weightbar")]
        [SerializeField] private Image _weightFillImage;
        
        [Header("Stamina Wheel")]
        [SerializeField] private CanvasGroup _wholeWheelCanvasGroup;
        [SerializeField] private Image _redWheel;
        [SerializeField] private Image _greenWheel;
        [SerializeField] private CanvasGroup _grabButton;
        
        [Header("Score")]
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _panelWinText;
        
        
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
            } ;
        }
        
        private void UpdateScoreUI()
        {
            if (_scoreText != null)
            {
                _scoreText.text = $"Score Total : {ScoreManager.instance.ScoreData.CurrentScore} points";
            }
        }
        
        private void UpdateScoreWinUI()
        {
            _panelWinText.text = "Ton Score\n " + $"{ScoreManager.instance.ScoreData.CurrentScore}\n" + "\n" + "Meilleur Score\n " + $"{ScoreManager.instance.ScoreData.BestScore}";
        }
    }
}
