using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Local.Integration.Scripts.MainMenu;
using Local.Integration.Scripts.SCORE;
using TMPro;
using UnityEngine;

public class ScoreDigits : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI _digits;
    [SerializeField] private ScoreData _scoreData;
    [SerializeField] private float _interpolationTime;
    [SerializeField] private AudioClip _scoreSound;

    private int _prevScore, _currScore;
    private bool _isTweening;
    
    // Update is called once per frame
    void Update() {
        _currScore = _scoreData.CurrentScore;
        _digits.text = _prevScore.ToString();
        
        if (_currScore != _prevScore && _isTweening == false) {
            StartCoroutine(TweenScore());
        }
    }

    private IEnumerator TweenScore() {
        _isTweening = true;
        SoundFXManager.instance.PlaySoundFXClip(_scoreSound, transform, .35f);
        DOTween.To(()=> _prevScore, _currScore => _prevScore = _currScore,
            _currScore, _interpolationTime).SetEase(Ease.OutCirc);
        yield return new WaitForSeconds(_interpolationTime);
        _isTweening = false;
    }
}
