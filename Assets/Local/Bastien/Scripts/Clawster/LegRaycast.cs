using System.Collections;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

/*  The origin offset issue was fixed today by the discovery of this addon :
              https://github.com/EdyJ/blender-to-unity-fbx-exporter
   Thanks to this GOATed dev for the script. NO MORE time wasted on rotations! */

public class LegRaycast : MonoBehaviour {
    [Header("GameObjects")]
    public Transform IKTarget;                  //GameObject of the IK Target
    
    [Header("Main Settings")]
    public float MaxDistance;                   //Maximum distance between to steps
    public float InterpolationTime;             //Time to interpolate between the 2 positions, in secs
    public float LegRaise;                      //Height of the step animation
    
    [Header("Fine tuning")] 
    [SerializeField] private Vector3 _overshootFac;             //Supplementary distance during Lerp
    [SerializeField] private AnimationCurve _interpolationCurve;//Curve followed by said interpolation

    [Header("Graphics")]
    [SerializeField] private ParticleSystem _stepParticles;
    
    private Vector3 _IKPos;                     //Leg position
    private Vector3 _currentHitPos;             //Current point that the raycast hits
    private RaycastHit _hit;                    //Entity containing results of the raycast
    
    // Update is called once per frame
    void Update() {
        //Cast the ray. Using Vector3.down guarantees that transform has no role in the direction.
        if (Physics.Raycast(transform.position, Vector3.down, out _hit, Mathf.Infinity)) {
            _currentHitPos = _hit.point;
        }
        
        //Is the raycast far away from the foot? Move
        if (Vector3.Distance(IKTarget.position, _currentHitPos) > MaxDistance) {
            
            StartCoroutine(Walk(InterpolationTime, _IKPos, _currentHitPos)); //Here we go
        } else {
            IKTarget.position = _IKPos;                     //Keep the foot at the same place
        }
    }

    //Animation coroutine
    IEnumerator WalkInterpolate(float dt, Vector3 start, Vector3 end) {
        Vector3 tempPos;        //Declared as (0,0,0).
        float pi = Mathf.PI;    //Self-explanatory.
        float dtNorm;           //Undeclared impl. NULL impl. (float) 0.

        //Curve-based interpolation
        for (dt = 0f; dt < InterpolationTime; dt += Time.deltaTime) {
            dtNorm = dt / InterpolationTime;                                        //Normalize dt (0,1) to prevent OF.
            tempPos = Vector3.Lerp(start, end, _interpolationCurve.Evaluate(dtNorm));//Use the curve to interpolate
            tempPos.y += Mathf.Sin(pi * dtNorm) * LegRaise;                         //Maybe too much, but will do.

            IKTarget.position = tempPos;        //Repeated assignment to apply the motion
            yield return null;                  //Use this. WaitForEndOfFrame() execs after all game logic, so RIP.
        }
        _IKPos = end; //Last assignment -- maybe redundant but ensures values were not modified by FP inaccuracy.
        if (_stepParticles != null)
        {
            _stepParticles.Play();
        }
    }

    IEnumerator Walk(float dt, Vector3 start, Vector3 end) {
        yield return StartCoroutine(WalkInterpolate(dt, start, end));
    }

    private void OnGUI() {
        Debug.DrawRay(transform.position, Vector3.down, Color.red);
    }
}
