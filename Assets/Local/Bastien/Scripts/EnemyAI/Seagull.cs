using System;
using System.Collections;
using BezierSolution;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

/* Seagull Behaviour
 A trio of chained coroutines determines the attack of the seagull
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
    private Vector3 _startPos, _endPos;
    private Vector3 _targetPos;
    private Vector3 _seagullAimPosition;
    private float defaultXRotation;
    
    [Header("Attack Animation")]
    [SerializeField] private Animator _seagullAnimator;
    private AnimationCurve _heightCurve;

    [Header("Complementary Graphics & Animation")]
    [SerializeField] private BezierLineRenderer _bezierLineRenderer;
    [SerializeField] private float _lineDrawingTime;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Wait());
    }

    // Update is called once per frame
    void Update() {
        
    }

    //Setup everything for the next move
    IEnumerator Setup() {
        _startPos  =  _start.transform.position;
        _endPos    =    _end.transform.position;
        _targetPos = _target.transform.position;                        //Center the area on Clawster's pos.
        transform.position = _targetPos;                                //Apply said position
        
        _attackAngle = Random.Range(0f, 360f);                          //Choose a random Y angle
        transform.rotation = Quaternion.Euler(0, _attackAngle, 0);      //Apply said angle

        defaultXRotation = _seagull.transform.rotation.eulerAngles.x;   //Save rotation from the editor
        
        _seagull.transform.position = _startPos;        //Reset Seagull
        _seagull.transform.rotation = quaternion.LookRotation(_targetPos, transform.up);
        _seagullAimPosition = _targetPos;               //Set target
        _seagullAimPosition.y += _seagullHeight;        //Add padding height to prevent clipping
        _seagullAnimator.SetBool("IsSoaring", true);    //Reset animation trigger
        
        yield return StartCoroutine(Attack());
    }

    IEnumerator Attack() {
        //First DOTween, draws the danger circle around the player
        _bezierLineRenderer.SplineSampleRange = new Vector2(0f, 0f);    //Mask the spline
        DOTween.To(()=> _bezierLineRenderer.SplineSampleRange, x =>     //Draw said spline
                        _bezierLineRenderer.SplineSampleRange = x,
                        new Vector2(0f, 1f), _lineDrawingTime).SetEase(Ease.InOutSine);
        
        //While this is completing, define the seagull trajectory
        Vector3[] waypoints = {_startPos, _seagullAimPosition, _endPos };
        
        //Seagull movement. It follows a path passing by the player's location
        DG.Tweening.Sequence anim = DOTween.Sequence();
        
        _seagullAnimator.SetBool("IsSoaring", false);
        anim.Join(_seagull.transform.DOPath(waypoints, 3f, PathType.CatmullRom, gizmoColor:Color.red).SetEase(Ease.InOutSine));
        // anim.Join(_seagull.transform.DOMoveZ(_targetPosition.z, 1.5f).SetEase(Ease.InQuart));
        // anim.Join(_seagull.transform.DOMoveX(_targetPosition.x, 1.5f).SetEase(Ease.InQuart));
        // anim.Join(_seagull.transform.DOMoveY(_targetPosition.y, 1.5f).SetEase(Ease.Linear));
        // anim.Append(_seagull.transform.DOMoveZ(_end.transform.position.z, 1.5f).SetEase(Ease.OutQuart));
        // anim.Join(_seagull.transform.DOMoveX(_end.transform.position.x, 1.5f).SetEase(Ease.OutQuart));
        // anim.Join(_seagull.transform.DOMoveY(_end.transform.position.y, 1.5f).SetEase(Ease.Linear));
        // anim.Append(_seagull.transform.DOLocalRotate((Quaternion.LookRotation(_endPos, transform.up).eulerAngles), 1.5f).SetEase(Ease.Linear));
        anim.Play();
        yield return new WaitForSeconds(1f);
        _seagullAnimator.SetBool("IsSoaring", true);
        yield return new WaitForSeconds(2.5f);
        
        yield return StartCoroutine(Wait());
    }

    //Simply wait a random amount of time for the next move
    IEnumerator Wait() {
        _attackTimer = Random.Range(_minWaitTime, _maxWaitTime);
        yield return new WaitForSeconds(_attackTimer);
        yield return StartCoroutine(Setup());
    }

    private void OnGUI() {
        Debug.DrawRay(_seagull.transform.position, transform.forward * 100f, Color.red);
    }
}
