using System;
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
    public GameObject[] legCubes;       //Cube objects corresponing to above targets

    [Header("Movement & Rotation")]
    public bool enableBodyRotation = false;
    public bool enableMovementRotation = false;
    public bool rigidBodyController;

    [Header("Leg Motion")]
    public float moveDistance = 0.7f;       //Maximum distance between 2 steps
    public float stepHeight = 0.2f;         //Maximum height that can be climbed
    public float jitterCutoff = 0f;         //I have no idea
    public float legSmoothing = 8f;         //I have no idea for the moment
    public float bodySmoothing = 8f;        //I have no idea for the moment
    public float overStepFac = 4f;          //I have no idea for the moment

    //Private garbage (I showed it to fread, IHO it's disgusting)
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

        }
    }

    private void FixedUpdate()
    {
        _velocity = Crab.transform.position - _lastCrabPos; //Set velocity to the delta between the positions
        _velocity = (_velocity + bodySmoothing * _lastVelocity) / (bodySmoothing + 1f);
    }
}

