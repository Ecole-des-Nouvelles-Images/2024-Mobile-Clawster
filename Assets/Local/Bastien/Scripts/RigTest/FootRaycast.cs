
using System;
using UnityEngine;
using UnityEngine.Serialization;

public class FootRaycast : MonoBehaviour
{
    [SerializeField] private Transform _raycastOrigin;
    [SerializeField] private Transform _target;
    [SerializeField] private float _maxDistance;
    [SerializeField] private float _animationTime = 1;
    [SerializeField] private float _yOffset = 5;
    
    public bool OnMove;
    public bool CanMove;
    
    
    
    
    private Vector3 _currentRaycastPosition;
    private Vector3 _currentTargetPosition;
    
    private Vector3 _startPosition;
    private Vector3 _offset;
    private Vector3 _endPos;
    
    private float _timer;
    
    // Update is called once per frame
    void Start() {
        Cast();
        _startPosition = _currentTargetPosition;
    }

    private void Cast()
    {
        RaycastHit hit;
        if (Physics.Raycast(_raycastOrigin.position, _raycastOrigin.forward, out hit)) {
            _currentRaycastPosition = hit.point;
            _target.position = hit.point; 
            Debug.DrawLine(_raycastOrigin.position , hit.point, Color.red);
        }
    }

    private void Update() {
        
        
        Cast();
        Debug.DrawLine(_currentRaycastPosition , _target.position , Color.green);

        
       
        
        if(OnMove) DoMove();
        else {
            if (Vector3.Distance(_currentRaycastPosition, _currentTargetPosition) > _maxDistance)
            {
                StartLegAnimation( _currentRaycastPosition);
            }
        }
            
        _target.transform.position = _currentTargetPosition;
    }

    private void StartLegAnimation(Vector3 endPos) {
        _endPos = endPos;
        _timer = 0;
        OnMove = true;
        _startPosition = _currentTargetPosition;
        _offset = Vector3.Lerp(_startPosition , _endPos, 0.5f)+new Vector3(0,_yOffset,0);
    }

    private void DoMove() {
        _timer += Time.deltaTime;
        float t = _timer / _animationTime;
        
        
        Vector3 a = Vector3.Lerp(_startPosition, _offset, t);
        Vector3 b = Vector3.Lerp(_offset, _endPos, t);
        Vector3 c = Vector3.Lerp(a, b, t);

        _currentTargetPosition = c;

        if (_timer >= _animationTime) {
            _currentTargetPosition = _endPos;
            OnMove = false;
        }



    }
}

public class LegManager : MonoBehaviour
{
    [SerializeField] private FootRaycast[] _legs;
    [SerializeField] private int MaxMo


    private void Update()
    {

        int movingLegs = 0;
        foreach (var leg in _legs)
        {
            if (leg.OnMove)
            {
                movingLegs++;
            }
        }
    }
}
