using System.Collections;
using UnityEngine;

/* Moving all of the grabbing logic to this script */

public class ItemGrab : MonoBehaviour {
    public GameObject[] HandAim; //Hands with index 0 and 1
    public Collider GrabCollider; //Actual collision of the grab zone
    public Camera PlayerCam; //Main game camera

    public float InterpolationTime; //
    public AnimationCurve InterpolationCurve; //Curve used by interpolation

    private Ray _ray; //Raycast
    private RaycastHit _hit; //Hit info
    private bool _inRange; //Self-explanatory   
    private GameObject _hand; //Soon to be selected hand
    private GameObject _hitObj; //Object hit by raycast

    private void Start() {

    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            _inRange = true;
        }
    }

    private void OnTriggerStay(Collider other) {
        if (!_inRange) return; //Forces items to be in the trigger
        
        _ray = PlayerCam.ScreenPointToRay(Input.mousePosition); //Sets the ray to be fired from the camera

        if (!_hit.collider.CompareTag("Item")) return;

        if (Physics.Raycast(_ray, out _hit, Mathf.Infinity)) {  //Did a raycast happen ?
            Debug.Log("Click detected, in range, hit an item");
            _hitObj = _hit.collider.gameObject;

            if (!_hitObj.CompareTag("Item")) return;
            float dist0 = Vector3.Distance(_hitObj.transform.position, HandAim[0].transform.position);
            float dist1 = Vector3.Distance(_hitObj.transform.position, HandAim[1].transform.position);

            _hand = (dist0 < dist1) ? HandAim[0] : HandAim[1];

            StartCoroutine(GrabInterpolate(InterpolationTime, _hitObj.transform.position, _hand.transform.position));
        }
    }

    IEnumerator GrabInterpolate(float dt, Vector3 start, Vector3 end) {
        Vector3 tempPos; //Declared as (0,0,0).
        float pi = Mathf.PI; //Self-explanatory.
        float dtNorm; //Undeclared impl. NULL impl. (float) 0.

        for (dt = 0f; dt < InterpolationTime; dt += Time.deltaTime) {
            dtNorm = dt / InterpolationTime;
            tempPos = Vector3.Lerp(start, end, InterpolationCurve.Evaluate(dtNorm));
            _hand.transform.position = tempPos;
            yield return null;
        }
    }
}