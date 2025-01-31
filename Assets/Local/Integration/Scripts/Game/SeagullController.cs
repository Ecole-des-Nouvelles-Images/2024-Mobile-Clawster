using BezierSolution;
using DG.Tweening;
using UnityEngine;
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

    [Header("Attack Animation"), SerializeField]
     private Animator _seagullAnimator;

    [Header("Complementary Graphics & Animation"), SerializeField]
     private BezierLineRenderer _bezierLineRenderer;

    [SerializeField] private float _lineDrawingTime;
    private float _drawT;
    private float _truceT;
    private AnimationCurve _heightCurve;
    private Vector3 _seagullAimPosition;
    private Vector3 _startPos, _endPos, _targetPos;

    private int _stateID;

    private bool _isInLooping;

    private void OnEnable() {
        Setup();
        GetComponent<BezierSpline>().loop = true;
    }


    //State 1, takes only 1 frame to execute
    private void Setup() {
        transform.position = _target.transform.position; //Center animation on Clawster
        _attackAngle = Random.Range(0f, 360f); //Choose a random Y angle
        transform.rotation = Quaternion.Euler(0, _attackAngle, 0); //Apply said angle
        _startPos = _start.transform.position; //Place start and end markers
        _endPos = _end.transform.position;
        _targetPos = _target.transform.position;
        _seagullAimPosition = _targetPos;
        _seagull.transform.position = _startPos; //Reset    Seagull to start of tween
        _lookTarget.transform.position = _startPos; //Reset LookTarget to start of tween
        _seagullAimPosition.y += _seagullHeight; //Add padding height to prevent clipping
        _seagullAnimator.SetBool("IsSoaring", true); //Reset animation trigger
        DrawCircle();
    }

    //State 2
    private void DrawCircle() {
        if (_isInLooping == false) {
            _bezierLineRenderer.SplineSampleRange = new Vector2(0f, 0f); //Mask the spline
            DOTween.To(() => _bezierLineRenderer.SplineSampleRange, x => //Draw said spline
                            _bezierLineRenderer.SplineSampleRange = x, new Vector2(0f, 1f), _lineDrawingTime).SetEase(Ease.InOutSine);
        }
        
    }

    //State 3
    private void Attack() {
        //While this is completing, define the seagull trajectory
        Vector3[] waypoints = { _startPos, _seagullAimPosition, _endPos };

        _seagull.SetActive(true);

        Sequence anim = DOTween.Sequence();
        _seagullAnimator.SetBool("IsAttacking", true);

        anim.Join(_lookTarget.transform.DOPath(waypoints, _attackTime, PathType.CatmullRom, gizmoColor: Color.red)
            .SetEase(Ease.InOutSine));
        anim.Insert(_seagullDelay, _seagull.transform
            .DOPath(waypoints, _attackTime, PathType.CatmullRom, gizmoColor: Color.blue)
            .SetEase(Ease.InOutSine)).OnComplete(WaitForNextAttack);
        anim.Play();

        //Block the execution of other functions using a Coroutine lasting as long as the tween
        /*for (float i = 0; i < _attackTime; i += Time.deltaTime) {
            _seagull.transform.LookAt(_lookTarget.transform.position);
            Debug.DrawRay(_seagull.transform.position, _seagull.transform.forward, Color.blue, 1f);
            break;
        }*/

        //Change animation
        _seagullAnimator.SetBool("IsAttacking", false);

        _bezierLineRenderer.SplineSampleRange = new Vector2(0f, 0f);
        _seagull.SetActive(false);
        WaitForNextAttack();
    }

    //State 4
    private void WaitForNextAttack() {
        if (PlayerPresence) {
            _truceT = Random.Range(_minWaitTime, _maxWaitTime);
            Setup();
        }
    }
}