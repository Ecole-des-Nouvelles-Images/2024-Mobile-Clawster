using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Local.Integration.Scripts.Game
{
    public class Countdown : MonoBehaviour
    {

        [SerializeField] private TMP_Text _gameTimerText;
        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private AnimationCurve _easeInOut;

        public Vector3 InitialScale;

        private IEnumerator CountdownCoroutine(int time, string str)
        {
            if (time > 0)
            {
                _gameTimerText.text = time.ToString();
            }
            else
            {
                _gameTimerText.text = str;
            }

            _gameTimerText.transform.localScale = InitialScale;
            _gameTimerText.transform.DOScale(Vector3.one, 1.1f).SetEase(_easeInOut);

            yield return new WaitForSeconds(1.1f);

            if (time == 0)
            {
                yield return new WaitForSeconds(0.01f); 
                _gameTimerText.enabled = false;
                _scoreText.enabled = false;
                GameManager.instance.HasStarted = true;
                yield break;
            }

            time--;
            StartCoroutine(CountdownCoroutine(time, str));
        }

        private void Start()
        {
            StartCoroutine(CountdownCoroutine(GameManager.instance.CountdownTime, GameManager.instance.StartText));
        }
    }
}