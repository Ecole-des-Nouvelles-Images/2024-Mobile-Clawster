using System.Collections;
using System.Collections.Generic;
using Local.Integration.Scripts.Game;
using TMPro;
using UnityEngine;

public class TimerDigits : MonoBehaviour {
    
    [SerializeField] private TextMeshProUGUI _timerText;
    private float _rawTimer;
    private int _minutes;
    private int _seconds;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {
        if (_rawTimer <= 0 && GameManager.instance.HasStarted) {
            _rawTimer = 0;
        } else {
            _rawTimer = GameManager.instance.GameTime - GameManager.instance.ElapsedTime;
        }
        
        _minutes = Mathf.FloorToInt(_rawTimer / 60);
        _seconds = Mathf.FloorToInt(_rawTimer % 60);
        _timerText.text = _minutes.ToString("0") + ":" + _seconds.ToString("00");
        
    }
}
