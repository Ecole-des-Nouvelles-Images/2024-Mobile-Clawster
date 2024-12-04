using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CrabAnimationController : MonoBehaviour
{
    /*Slightly modified version of the script,
    in order to clean some of the terrible variable
    names, and make the thing more digestable overall*/

    [Header("GameObjects")] 
    public GameObject Crab;             //Clawster himself!
    public GameObject[] LegTargets;     //Target positions of the IK chain
    public GameObject[] LegCubes;       //Cube objects corresponding to above targets
    public GameObject[] ArmTargets;     //Target objects for the arms to reach
    
    [Header("Movement & Rotation")]
    public bool EnableBodyRotation = false;
    public bool EnableMovementRotation = false;
    public bool RigidBodyController;

    [Header("Leg Motion")]
    public float moveDistance = 0.7f;       //Maximum distance between 2 steps
    public float stepHeight = 0.15f;        //Maximum height that can be climbed
    public float jitterCutoff = 0f;         //I have no idea
    public float legSmoothing = 8f;         //Animation smoothing factor
    public float bodySmoothing = 8f;        //Animation smoothing factor
    public float overStepFac = 4f;          //I have no idea for the moment
    public int stepWaitTime = 1;
    public AnimationCurve interpolationCurve;
    
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

    private void Start() {
        _lastBodyUp = transform.up;
        
        _currLegPos = new Vector3[LegTargets.Length]; //Making sure both arrays have the same size
        _origLegPos = new Vector3[LegTargets.Length]; //Repeat for other array

        for (int i = 0; i < LegTargets.Length; i++) {
            _currLegPos[i] = LegTargets[i].transform.position; //Assign pos from the target

            if (_currLeg) {
                _oppoMoveInd.Add(i + 1);
                _currLeg = false;
            }
            else if (!_currLeg) {
                _oppoMoveInd.Add(i - 1);
                _currLeg = true;
            }
            
            _lastCrabPos = Crab.transform.position;
            RotateBody();
        }
    }

    private void FixedUpdate() {
        _velocity = Crab.transform.position - _lastCrabPos; //Set velocity to the delta between the positions
        _velocity = (_velocity + bodySmoothing * _lastVelocity) / (bodySmoothing + 1f);

        MoveLegs();
        RotateBody();
        
        _lastCrabPos = Crab.transform.position;
        _lastVelocity = _velocity;
    }

    void MoveLegs() {
        if (!EnableMovementRotation) return;
        for (int i = 0; i < LegTargets.Length; i++) {
            if (Vector3.Distance(LegTargets[i].transform.position, LegCubes[i].transform.position) >= moveDistance) {
                if (!_nextMoveInd.Contains(i) && !_currMoveInd.Contains(i)) _nextMoveInd.Add(i);
                
            } else if (!_currMoveInd.Contains(i)) {
                LegTargets[i].transform.position = _origLegPos[i];
            }
        }

        if (_nextMoveInd.Count == 0 || _currMoveInd.Count != 0) return;
        Vector3 targetPos = LegCubes[_nextMoveInd[0]].transform.position + Mathf.Clamp(_velocity.magnitude * overStepFac, 0f, 1.5f)
            * (LegCubes[_nextMoveInd[0]].transform.position - LegTargets[_nextMoveInd[0]].transform.position) + _velocity * overStepFac;
        StartCoroutine(Step(_nextMoveInd[0], targetPos, false));
    }

    IEnumerator Step(int index, Vector3 moveTo, bool isOpp) {
        if(!isOpp) MoveOppLeg(_oppoMoveInd[index]);
        if(_nextMoveInd.Contains(index)) _nextMoveInd.Remove(index);
        if(!_currMoveInd.Contains(index)) _currMoveInd.Add(index);
        
        Vector3 startPos = _origLegPos[index];

        for (int i = 1; i <= legSmoothing; ++i) {
            LegTargets[index].transform.position = Vector3.Lerp(startPos, moveTo + new Vector3(0, stepHeight, 0), interpolationCurve.Evaluate(i / legSmoothing));
            yield return new WaitForEndOfFrame();  //Waits 1 frame
        }
        
        _origLegPos[index] = moveTo;
        
        for (int i = 1; i <= stepWaitTime; ++i) yield return null;
        if (_currMoveInd.Contains(index)) _currMoveInd.Remove(index);
    }

    private void MoveOppLeg(int index) {
        Vector3 targetPosition = LegCubes[index].transform.position + Mathf.Clamp(_velocity.magnitude * overStepFac, 0.0f, 1.5f)
            * (LegCubes[index].transform.position - LegTargets[index].transform.position) + _velocity * overStepFac;
        StartCoroutine(Step(index, targetPosition, true));
    }

    private void RotateBody() {
        if (!EnableBodyRotation || _velocity == Vector3.zero) return;
        
        Vector3 v1 = LegTargets[0].transform.position - LegTargets[7].transform.position;
        Vector3 v2 = LegTargets[4].transform.position - LegTargets[3].transform.position;
        Vector3 normal = Vector3.Cross(v1, v2).normalized;
        
        transform.rotation = Quaternion.LookRotation(_velocity);
        
        //Vector3 up = Vector3.Lerp(_lastBodyUp, normal, 1f / bodySmoothing);
        //transform.up = up;

        //transform.rotation = Quaternion.LookRotation(transform.parent.forward, up);
        _lastBodyUp = transform.up;
    }
    
    public IEnumerator Grab(GameObject target) {
        //Determine which arm should move to get the item based on calculating distances
        int id = (Vector3.Magnitude(target.transform.position - ArmTargets[0].transform.position)) <
                 (Vector3.Magnitude(target.transform.position - ArmTargets[1].transform.position)) ? 0 : 1;
        
        //The following code has a bug when Clawster wants to grab an object behind him
        Vector3 startLocalPos = ArmTargets[id].transform.localPosition;     //Save start position
        Vector3 targetLocalPos = new Vector3(
            target.transform.position.x - ArmTargets[id].transform.position.x,
            target.transform.position.y - ArmTargets[id].transform.position.y,
            target.transform.position.z - ArmTargets[id].transform.position.z);
        
        ArmTargets[id].transform.localPosition = targetLocalPos;      //Position test
        
        for (int i = 0; i < 20; i++) {
            ArmTargets[id].transform.localPosition =
                Vector3.Lerp(ArmTargets[id].transform.localPosition, startLocalPos, i / 20f);
            yield return new WaitForEndOfFrame();
        }
    }
    
    public void GrabAndRemoveObject(GameObject target) {
        StartCoroutine(Grab(target));
    }
    
     private void OnGUI() {
         GUI.skin.label.fontSize = 32; GUI.skin.box.fontSize = 32;
         GUI.Box(new Rect(20, 20, 300, 40), _velocity.magnitude.ToString());
         
         Debug.DrawRay(transform.position, _velocity, Color.blue, 1f, false);
     }

}

