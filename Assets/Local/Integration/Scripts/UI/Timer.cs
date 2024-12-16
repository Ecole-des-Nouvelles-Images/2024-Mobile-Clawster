using System.Collections;
using System.Collections.Generic;
using Local.Integration.Scripts.Game;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private Image _UItimer;
    
    private float _rawTimer;
    private float _fill;
    private int _minutes, _seconds;
    
    // Update is called once per frame
    void Update()
    {
        _rawTimer = _gameManager.RemainingTime;

        _fill = _rawTimer / _gameManager.GameplayTime;
        _minutes = Mathf.FloorToInt(_rawTimer / 60);
        _seconds = Mathf.FloorToInt(_rawTimer % 60);
        
        _UItimer.fillAmount = _fill;
    }
}
