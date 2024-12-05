using System.Collections;
using DG.Tweening;
using UnityEngine;

/* Moving all of the grabbing logic to this script */

public class ItemGrab : MonoBehaviour {
    public GameObject[] HandAim; //Hands with index 0 and 1
    public Collider GrabCollider; //Actual collision of the grab zone
    public Camera PlayerCam; //Main game camera

    public float InterpolationTime; //
    public AnimationCurve InterpolationCurve; //Curve used by interpolation
    public bool IsGrabbing { get; private set; }

    private Ray _ray; //Raycast
    private RaycastHit _hit; //Hit info
    private GameObject _hand; //Soon to be selected hand
    private GameObject _hitObj; //Object hit by raycast

    private void Update() {
        _ray = Camera.main.ScreenPointToRay(Input.mousePosition); //Sets the ray to be fired from the camera
    }

    private void OnTriggerStay(Collider other) {
        if (Input.GetMouseButtonDown(0)) {                      //Was the mouse pressed? Raycast
            Physics.Raycast(_ray, out _hit, Mathf.Infinity);    //Did a raycast happen?

            _hitObj = _hit.collider.gameObject;
            if (_hitObj.CompareTag("Item")) {
                IsGrabbing = true;
                float dist0 = Vector3.Distance(_hitObj.transform.position, HandAim[0].transform.position);
                float dist1 = Vector3.Distance(_hitObj.transform.position, HandAim[1].transform.position);

                _hand = (dist0 < dist1) ? HandAim[0] : HandAim[1];
                Debug.Log(_hand.name);

                StartCoroutine(GrabInterpolateDT(InterpolationTime, _hitObj.transform.position,
                    _hand.transform.position));
            }
        }
    }
    
    /*IEnumerator GrabInterpolate(float dt, Vector3 start, Vector3 end) {
        Debug.Log("Coroutine started");
        Vector3 tempPos; //Declared as (0,0,0).
        float pi = Mathf.PI; //Self-explanatory.
        float dtNorm; //Undeclared impl. NULL impl. (float) 0.

        for (dt = 0f; dt < InterpolationTime; dt += Time.deltaTime) {
            dtNorm = dt / InterpolationTime;
            tempPos = Vector3.Lerp(end, start, InterpolationCurve.Evaluate(Mathf.Sin(dtNorm * pi)));
            _hand.transform.position = tempPos;
            yield return null;
        }
        IsGrabbing = false;
    }*/

    IEnumerator GrabInterpolateDT(float dt, Vector3 start, Vector3 end) {
        Debug.Log("DOTween Coroutine started");
        
        Sequence seq = DOTween.Sequence();
        seq.Join(_hand.transform.DOPunchPosition(end, InterpolationTime - .1f,  8, 0.5f));
        seq.Join(_hand.transform.DOPunchRotation(end, InterpolationTime - .1f, 10, 0.7f));
        seq.Join(_hand.transform.DOPunchScale(   end, InterpolationTime - .1f,  4, 0.3f));
        seq.Play();
        for (dt = 0f; dt < InterpolationTime; dt += Time.deltaTime) yield return null; //Ensures Coroutine behaviour
        
        IsGrabbing = false; //Maintains Clawster in position
    }
}