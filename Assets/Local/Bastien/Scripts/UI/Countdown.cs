using System;
using System.Collections;
using System.Security.Cryptography;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class Countdown : MonoBehaviour {

    public GameManager gm;
    
    [SerializeField] private TMP_Text _gameTimerText;
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private AnimationCurve _easeInOut;
    
    public Vector3 InitialScale;
    public Vector3 InitialRotation;

    private IEnumerator CountdownCoroutine(int time, string str) {
        if (time >= 1) 
            _gameTimerText.text = time.ToString();
        else
            _gameTimerText.text = str;
        
        _gameTimerText.transform.localScale = InitialScale;
        _gameTimerText.transform.DOScale(Vector3.one, 1.1f).SetEase(_easeInOut);

        if (time == 0) {
            gm.GameStarted = true;
            Debug.Log("Game Started");
            yield break;
        }
        
        yield return new WaitForSeconds(1.1f);
        time--;
        StartCoroutine(CountdownCoroutine(time, str));
    }
    // Update is called once per frame
    private void Start() {
        StartCoroutine(CountdownCoroutine(gm.CountdownTime, gm.StartText));
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            
            StartCoroutine(CountdownCoroutine(gm.EndTime, gm.EndText));
        }
    }
}
