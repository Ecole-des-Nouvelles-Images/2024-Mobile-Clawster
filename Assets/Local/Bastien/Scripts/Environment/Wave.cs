using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;

public class Wave : MonoBehaviour
{
    [SerializeField] private Vector3 _startPos, _endPos;
    [SerializeField] private float _gameTime;

    [SerializeField] private List<GameObject> _waveChildren;
    
    
    // Start is called before the first frame update
    void Start()
    {
       transform.position = _startPos;
       transform.DOMove(_endPos, _gameTime).SetEase(Ease.InOutQuad);
    }

    // Update is called once per frame
    void Update() {
        
    }
}
