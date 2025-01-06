using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Local.Integration.Scripts.Game
{
    public class Countdown : MonoBehaviour
    {
        [SerializeField] private TMP_Text _gameTimerText;
        [SerializeField] private AnimationCurve _easeInOut;
        [SerializeField] private Vector3 _initialScale;
        private bool _isCoroutineRunning;

        private IEnumerator CountdownCoroutine(int time, string str, bool end) {
            _gameTimerText.enabled = true;
            _isCoroutineRunning = true;
            
            if (time > 0) {
                _gameTimerText.text = time.ToString();
            } else {
                _gameTimerText.text = str;
            }
            
            _gameTimerText.transform.localScale = _initialScale;
            _gameTimerText.transform.DOScale(Vector3.one, .95f).SetEase(_easeInOut);

            yield return new WaitForSeconds(1f);

            if (time == 0) {
                yield return new WaitForSeconds(.01f);

                if (end) {
                    _isCoroutineRunning = false;
                    yield break;
                }
                
                GameManager.instance.HasStarted = true;
                _isCoroutineRunning = false;
                yield break;
            }
            time--;
            StartCoroutine(CountdownCoroutine(time, str, end));
        }

        private void Start() {
            StartCoroutine(CountdownCoroutine(GameManager.instance.StartTime, GameManager.instance.StartText, false));
        }

        private void Update() {
            if (GameManager.instance.ElapsedTime > (GameManager.instance.GameTime - GameManager.instance.EndTime) && _isCoroutineRunning == false)
                StartCoroutine(CountdownCoroutine(GameManager.instance.EndTime, GameManager.instance.EndText, true));
        }
    }
}