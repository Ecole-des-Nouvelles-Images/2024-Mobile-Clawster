using System;
using System.Collections;
using System.Collections.Generic;
using BezierSolution;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

/* Seagull Behaviour
 A trio of chained coroutines determines the attack of the seagull
 and the time between them*/

public class SeagullController : MonoBehaviour {
    [Header("Essential Settings")]
    public bool PlayerPresence;
    [SerializeField] private GameObject _target;
    [SerializeField] private GameObject _seagull;
    [SerializeField] private GameObject _lookTarget;
    [SerializeField] private GameObject _start, _end;
    [SerializeField] private float _attackTime;
    [SerializeField] private float _seagullDelay;
    [SerializeField] private float _attackAngle;
    [SerializeField] private float _seagullHeight;
    [SerializeField] private float _minWaitTime, _maxWaitTime;
    
    [Header("Attack Animation")]
    [SerializeField] private Animator _seagullAnimator;
    private AnimationCurve _heightCurve;

    [Header("Complementary Graphics & Animation")]
    [SerializeField] private BezierLineRenderer _bezierLineRenderer;
    [SerializeField] private float _lineDrawingTime;
    
    private float _truceTimer;
    private Vector3 _startPos, _endPos, _targetPos;
    private Vector3 _seagullAimPosition;
    
    //Setup everything for the next move
    private void Setup() {
        Debug.Log("Setup");
        transform.position = _target.transform.position;            //Center animation on Clawster
        _attackAngle = Random.Range(0f, 360f);                      //Choose a random Y angle
        transform.rotation = Quaternion.Euler(0, _attackAngle, 0);  //Apply said angle
        _startPos  =  _start.transform.position;                    //Place start and end markers
        _endPos    =    _end.transform.position;                    
        _targetPos = _target.transform.position;
        _seagullAimPosition = _targetPos;
        _seagull.transform.position = _startPos;                    //Reset    Seagull to start of tween
        _lookTarget.transform.position = _startPos;                 //Reset LookTarget to start of tween
        _seagullAimPosition.y += _seagullHeight;                    //Add padding height to prevent clipping
        _seagullAnimator.SetBool("IsSoaring", true);                //Reset animation trigger
        StartCoroutine(Attack());
    }

    private IEnumerator Attack() {
        //First DOTween, draws the danger circle around the player
        _bezierLineRenderer.SplineSampleRange = new Vector2(0f, 0f);    //Mask the spline
        DOTween.To(()=> _bezierLineRenderer.SplineSampleRange, x =>     //Draw said spline
                        _bezierLineRenderer.SplineSampleRange = x,
                        new Vector2(0f, 1f), _lineDrawingTime).SetEase(Ease.InOutSine);
        yield return new WaitForSeconds(_lineDrawingTime);
        
        //While this is completing, define the seagull trajectory
        Vector3[] waypoints = {_startPos, _seagullAimPosition, _endPos};
        
        _seagull.SetActive(true);
        
        DG.Tweening.Sequence anim = DOTween.Sequence();
        _seagullAnimator.SetBool("IsAttacking",true);
        
        anim.Join(_lookTarget.transform.DOPath(waypoints, _attackTime, PathType.CatmullRom, gizmoColor: Color.red)
            .SetEase(Ease.InOutSine));
        anim.Insert(_seagullDelay, _seagull.transform.DOPath(waypoints, _attackTime, PathType.CatmullRom, gizmoColor:Color.blue)
            .SetEase(Ease.InOutSine));
        anim.Play();
        
        //Block the execution of other functions using a Coroutine lasting as long as the tween
        for (float i = 0; i < _attackTime; i += Time.deltaTime) {
            _seagull.transform.LookAt(_lookTarget.transform.position);
            yield return new WaitForEndOfFrame();
        }
        
        //Change animation
        _seagullAnimator.SetBool("IsAttacking", false);
        yield return new WaitForSeconds(1f);
        
        _bezierLineRenderer.SplineSampleRange = new Vector2(0f, 0f);
        _seagull.SetActive(false);
        yield return StartCoroutine(WaitForNextAttack());
    }

    //Simply wait a random amount of time for the next move
    private IEnumerator WaitForNextAttack() {
        if (!PlayerPresence) yield break;
        _truceTimer = Random.Range(_minWaitTime, _maxWaitTime);
        yield return new WaitForSeconds(_truceTimer);
        Setup();
    }

    public void Reset() {
        StopAllCoroutines();
    }

    public void DoCycle() {
        StartCoroutine(WaitForNextAttack());
    }
}
