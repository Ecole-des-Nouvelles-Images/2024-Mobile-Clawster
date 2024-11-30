using System.Collections;
using UnityEngine;

/*          .
 *         / \                     W  A  R  N  I  N  G
 *        / _ \              It appears that exporting to FBX
 *       / |#| \          using Vincent's technique can slightly
 *      /  |#|  \          offset the origin. It is here fixed
 *     /    V    \        with a saefty margin for the Raycasts
 *    /           \
 *   /     [@]     \    Hoping to find an in-editor soultion to it.
 *  /_______________\
 *  
*/

public class LegRaycast : MonoBehaviour
{
    public Transform IKTarget;                  //GameObject of the IK Target
    public float MaxDistance;                   //Maximum distance between to steps
    public float InterpolationTime;             //Time taken to interpolate between the 2 positions, in seconds
    public float LegRaise;                      //Height of the step animation
    public AnimationCurve InterpolationCurve;   //Curve followed by said interpolation
    
    private Vector3 _IKPos;                     //Leg position
    private Vector3 _currentHitPos;             //Current point that the raycast hits
    private RaycastHit _hit;                    //Entity containing results of the raycast
    private float _yMargin = 0.05f;             //Small safety padding for the raycast
    
    // Update is called once per frame
    void Update() {
        //Cast the ray. Using Vector3.down guarantees that transform has no role in the direction.
        if (Physics.Raycast(transform.position, Vector3.down, out _hit, 2f)) {
            Debug.Log("RCH");
            transform.position = new Vector3(_hit.point.x, _hit.point.y + _yMargin, _hit.point.z);
            _currentHitPos = _hit.point; 
        };
        
        //Is the raycast far away from the foot? Move
        if (Vector3.Distance(_IKPos, _currentHitPos) > MaxDistance) {
            StartCoroutine(Interpolate(InterpolationTime, _IKPos, _currentHitPos)); //Here we go
        } else {
            IKTarget.position = _IKPos;                         //Keep the foot at the same place
        }
    }

    //Animation coroutine
    IEnumerator Interpolate(float dt, Vector3 start, Vector3 end)
    {
        Vector3 tempPos; //Declared as (0,0,0).
        float pi = Mathf.PI; //Self-explanatory.
        float dtNorm; //Undeclared impl. NULL impl. (float) 0.

        //Curve-based interpolation
        for (dt = 0f; dt < InterpolationTime; dt += Time.deltaTime)
        {
            dtNorm = dt / InterpolationTime; //Normalize dt (0,1), prevents Evaluate() from overflowing.
            tempPos = Vector3.Lerp(start, end, InterpolationCurve.Evaluate(dtNorm)); //Use the curve to interpolate
            tempPos.y = Mathf.Sin(pi * dtNorm) * LegRaise; //Leg height uses sin(). Maybe overblown, but will do now.

            IKTarget.position = tempPos; //Repeated assignment to apply the motion
            yield return null; //Use this. WaitForEndOfFrame() execs after all game logic, so RIP.
        }

        _IKPos = end; //Last assignment -- maybe redundant but ensures values were not modified by FP inaccuracy.
    }

    private void OnGUI()
    {
        Debug.DrawRay(transform.position, Vector3.down, Color.red);
    }
}
