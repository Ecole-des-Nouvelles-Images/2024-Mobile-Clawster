using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabController : MonoBehaviour
{
    /*Slightly modified version of the script,
    in order to clean some of the terrible variable
    names, and make the thing more digestable overall*/

    [Header("GameObjects")] 
    public GameObject Crab;             //Clawster himself!
    public GameObject[] legTargets;     //Target positions of the IK chain
    public GameObject[] legCubes;       //Cube objects corresponding to above targets

    [Header("Movement & Rotation")]
    public bool enableBodyRotation = false;
    public bool enableMovementRotation = false;
    public bool rigidBodyController;

    [Header("Leg Motion")]
    public float moveDistance = 0.7f;       //Maximum distance between 2 steps
    public float stepHeight = 0.15f;        //Maximum height that can be climbed
    public float jitterCutoff = 0f;         //I have no idea
    public float legSmoothing = 8f;         //I have no idea for the moment
    public float bodySmoothing = 8f;        //I have no idea for the moment
    public float overStepFac = 4f;          //I have no idea for the moment
    public int stepWaitTime = 1;
    public float velocityCap = 6f;
    
    //Private garbage (I showed it to Fred, IHO it's disgusting)
    private Vector3 _velocity;
    private Vector3 _lastVelocity = Vector3.one;
    private Vector3 _lastCrabPos;
    private Vector3 _lastBodyUp;
    private Vector3[] _currLegPos;
    private Vector3[] _origLegPos;
    
    private List<int> _nextMoveInd = new List<int>();
    private List<int> _currMoveInd = new List<int>();
    private List<int> _oppoMoveInd = new List<int>();

    private bool _currLeg = true;
    private float _resetTimer = 0.5f;

    private void Start()
    {
        _lastBodyUp = transform.up;
        
        _currLegPos = new Vector3[legTargets.Length]; //Making sure both arrays have the same size
        _origLegPos = new Vector3[legTargets.Length]; //Repeat for other array

        for (int i = 0; i < legTargets.Length; i++)
        {
            _currLegPos[i] = legTargets[i].transform.position; //Assign pos from the target
            _currLegPos[i] = legTargets[i].transform.position; //Make values identical in both arrays

            if (_currLeg)
            {
                _oppoMoveInd.Add(i + 1);
                _currLeg = false;
            }
            else if (!_currLeg)
            {
                _oppoMoveInd.Add(i - 1);
                _currLeg = true;
            }
            
            _lastCrabPos = Crab.transform.position;

            RotateBody();
        }
    }

    private void FixedUpdate()
    {
        _velocity = Crab.transform.position - _lastCrabPos; //Set velocity to the delta between the positions
        _velocity = (_velocity + bodySmoothing * _lastVelocity) / (bodySmoothing + 1f);

        MoveLegs();
        RotateBody();
        
        _lastCrabPos = Crab.transform.position;
        _lastVelocity = _velocity;
    }

    void MoveLegs()
    {
        if (!enableMovementRotation) return;
        for (int i = 0; i < legTargets.Length; i++)
        {
            if (Vector3.Distance(legTargets[i].transform.position, legCubes[i].transform.position) >= moveDistance)
            {
                if (!_nextMoveInd.Contains(i) && !_currMoveInd.Contains(i)) _nextMoveInd.Add(i);
                
            } else if (!_currMoveInd.Contains(i))
            {
                legTargets[i].transform.position = _origLegPos[i];
            }
        }

        if (_nextMoveInd.Count == 0 || _currMoveInd.Count != 0) return;
        Vector3 targetPos = legCubes[_nextMoveInd[0]].transform.position + Mathf.Clamp(_velocity.magnitude * overStepFac, 0f, 1.5f) * (legCubes[_nextMoveInd[0]].transform.position - legTargets[_nextMoveInd[0]].transform.position) + _velocity * overStepFac;
        StartCoroutine(Step(_nextMoveInd[0], targetPos, false));
    }

    IEnumerator Step(int index, Vector3 moveTo, bool isOpp)
    {
        if(!isOpp) MoveOppLeg(_oppoMoveInd[index]);
        if(_nextMoveInd.Contains(index)) _nextMoveInd.Remove(index);
        if(!_currMoveInd.Contains(index)) _currMoveInd.Add(index);
        
        Vector3 startPos = _origLegPos[index];

        for (int i = 1; i <= legSmoothing; ++i)
        {
            legTargets[index].transform.position = Vector3.Lerp(startPos, moveTo + new Vector3(0, Mathf.Sin(i / (float)(legSmoothing + jitterCutoff) * Mathf.PI) * stepHeight, 0), (i / legSmoothing + jitterCutoff));
            yield return null;  //Waits 1 frame
        }
        
        _origLegPos[index] = moveTo;
        
        for (int i = 1; i <= stepWaitTime; ++i) yield return new WaitForFixedUpdate();
        if (_currMoveInd.Contains(index)) _currMoveInd.Remove(index);
    }

    private void MoveOppLeg(int index)
    {
        Vector3 targetPos = legCubes[index].transform.position + Mathf.Clamp(_velocity.magnitude * overStepFac, 0f, 1.5f) * (legCubes[index].transform.position - legTargets[index].transform.position) + _velocity * overStepFac;
        StartCoroutine(Step(index, targetPos, true));
    }

    private void RotateBody()
    {
        if (!enableBodyRotation) return;
        Vector3 v1 = legTargets[0].transform.position - legTargets[7].transform.position;
        Vector3 v2 = legTargets[4].transform.position - legTargets[3].transform.position;
        Vector3 normal = Vector3.Cross(v1, v2).normalized;
        Vector3 up = Vector3.Lerp(_lastBodyUp, normal, 1f / bodySmoothing);
        transform.up = up;

        if (!rigidBodyController) transform.rotation = Quaternion.LookRotation(transform.parent.forward, up);
        _lastBodyUp = transform.up;
    }
    private void OnGUI()
    {
        GUI.skin.label.fontSize = 32; GUI.skin.box.fontSize = 32;
        GUI.Box(new Rect(20, 20, 300, 40), _velocity.magnitude.ToString());
    }
}

