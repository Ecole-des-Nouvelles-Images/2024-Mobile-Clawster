using System.Collections;
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

    private void Start() {

    }

    private void Update() {
        if (PlayerCam != null)
        {
            _ray = Camera.main.ScreenPointToRay(Input.mousePosition); //Sets the ray to be fired from the camera
        }
    }

    private void OnTriggerStay(Collider other) {
        if (Input.GetMouseButtonDown(0)) {
            Physics.Raycast(_ray, out _hit, Mathf.Infinity);
            //Did a raycast happen ?
            _hitObj = _hit.collider.gameObject;
            if (_hitObj.CompareTag("Item")) {
                IsGrabbing = true;
                float dist0 = Vector3.Distance(_hitObj.transform.position, HandAim[0].transform.position);
                float dist1 = Vector3.Distance(_hitObj.transform.position, HandAim[1].transform.position);

                _hand = (dist0 < dist1) ? HandAim[0] : HandAim[1];
                Debug.Log(_hand.name);

                StartCoroutine(GrabInterpolate(InterpolationTime, _hitObj.transform.position,
                    _hand.transform.position));
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        
    }

    IEnumerator GrabInterpolate(float dt, Vector3 start, Vector3 end) {
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
    }
}