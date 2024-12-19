using System.Collections;
using System.Collections.Generic;
using Local.Integration.Scripts.Game;
using TMPro;
using UnityEngine;

public class EndScores : MonoBehaviour
{
    [SerializeField] private ScoreData _scoreData;
    [SerializeField] private TextMeshProUGUI _latestScore;
    [SerializeField] private TextMeshProUGUI _topScore;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _latestScore.text = _scoreData.CurrentScore.ToString();
        _topScore.text = _scoreData.BestScore.ToString();
    }
}
