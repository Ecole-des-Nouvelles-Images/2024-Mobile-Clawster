using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Local.Integration.Scripts.Game
{
    public class Countdown : MonoBehaviour {

        [SerializeField] private TMP_Text _gameTimerText;
        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private AnimationCurve _easeInOut;
    
        public Vector3 InitialScale;

        private IEnumerator CountdownCoroutine(int time, string str) {
            if (time >= 1) 
                _gameTimerText.text = time.ToString();
            else
                _gameTimerText.text = str;
        
            _gameTimerText.transform.localScale = InitialScale;
            _gameTimerText.transform.DOScale(Vector3.one, 1.1f).SetEase(_easeInOut);

            if (time == 0) {
                GameManager.instance.HasStarted = true;
                Debug.Log("Game Started");
                yield break;
            }
        
            yield return new WaitForSeconds(1.1f);
            time--;
            StartCoroutine(CountdownCoroutine(time, str));
        }
        private void Start() {
            StartCoroutine(CountdownCoroutine(GameManager.instance.CountdownTime, GameManager.instance.StartText));
        }
    }
}