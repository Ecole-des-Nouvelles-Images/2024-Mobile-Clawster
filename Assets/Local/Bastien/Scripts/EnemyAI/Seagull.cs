using System.Collections;
using System.Collections.Generic;
using BezierSolution;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Sequence = Unity.VisualScripting.Sequence;

/* Seagull Behaviour
 A pair of coroutines determines the attack of the seagull
 and the time between them*/

public class Seagull : MonoBehaviour {
    [Header("Essential Settings")]
    [SerializeField] private GameObject _target;
    [SerializeField] private GameObject _seagull;

    [SerializeField] private GameObject _start, _end;
    
    [SerializeField] private float _attackAngle;
    [SerializeField] private float _seagullHeight;
    [SerializeField] private float _minWaitTime, _maxWaitTime;

    private float _attackTimer;
    
    [Header("Attack Animation")] [SerializeField]
    private AnimationCurve _heightCurve;

    [Header("Complementary Graphics & Animation")]
    [SerializeField] private BezierLineRenderer _bezierLineRenderer;
    [SerializeField] private float _lineDrawingTime;
    
    /*[SerializeField] private 
    [SerializeField] private 
    [SerializeField] private 
    [SerializeField] private */ 
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Wait());
    }

    // Update is called once per frame
    void Update() {
        
    }

    IEnumerator Setup() {
        transform.position = _target.transform.position; //Center the area on Clawster
        
        _attackAngle = Random.Range(0f, 360f); //Choose a random Y angle for the attack
        transform.rotation = Quaternion.Euler(0, _attackAngle, 0);
        
        _seagull.transform.position = _start.transform.position;
        
        yield return StartCoroutine(Attack());
    }

    IEnumerator Attack() {
        _bezierLineRenderer.SplineSampleRange = new Vector2(0f, 0f);
        Vector3 targetPos = _target.transform.position;
        
        DOTween.To(()=> _bezierLineRenderer.SplineSampleRange, x =>
                        _bezierLineRenderer.SplineSampleRange = x,
                        new Vector2(0f, 1f), _lineDrawingTime);
        yield return new WaitForSeconds(_lineDrawingTime);
        
        Vector3[] waypoints = {_start.transform.position, targetPos, _end.transform.position };
        
        DG.Tweening.Sequence seq = DOTween.Sequence();
        _seagull.transform.DOPath(waypoints, 3f, PathType.CatmullRom, PathMode.Full3D).SetEase(Ease.InCubic);
        
        yield return new WaitForSeconds(3.5f);
        yield return StartCoroutine(Wait());
    }

    IEnumerator Wait() {
        _attackTimer = Random.Range(_minWaitTime, _maxWaitTime);
        yield return new WaitForSeconds(_attackTimer);
        
        Debug.Log($"Waited {_attackTimer} seconds");
        yield return StartCoroutine(Setup());
    }
}
