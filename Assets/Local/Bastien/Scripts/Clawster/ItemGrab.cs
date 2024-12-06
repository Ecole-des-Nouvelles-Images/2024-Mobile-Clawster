using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/* Moving all of the grabbing logic to this script */

public class ItemGrab : MonoBehaviour {
    public GameObject[] HandAim;    //Hands with index 0 and 1
    public Collider GrabCollider;   //Actual collision of the grab zone
    public Camera PlayerCam;        //Main game camera
    public Button GrabButton;       //UI Button to grab

    public List<GameObject> Stack;  //Stack that Clawster has on his back
    
    public float InterpolationTime; //
    public AnimationCurve InterpolationCurve; //Curve used by interpolation
    public bool IsGrabbing { get; private set; }
    
    
    private GameObject _hand;   //Soon to be selected hand
    private GameObject _hitObj; //Object hit by raycast
    private Item _hitItem;      //Item class belonging to the prefab

    private void Update() {     //Useless LMAO
    }

    private void OnTriggerEnter(Collider other) { 
        GrabButton.gameObject.SetActive(true);
        _hitObj = other.gameObject;                 //Get the object that is parent of the collider
        _hitItem = _hitObj.GetComponent<Item>();    //Get the stacked version of the item
    }

    private void OnTriggerExit(Collider other) {
        GrabButton.gameObject.SetActive(false);
        _hitObj = null;
    }

    public void Grab() {
        if (_hitObj.CompareTag("Item")) {   //Raycast part is gone, as we now use the UI to grab
            IsGrabbing = true;
            float dist0 = Vector3.Distance(_hitObj.transform.position, HandAim[0].transform.position);
            float dist1 = Vector3.Distance(_hitObj.transform.position, HandAim[1].transform.position);

            _hand = (dist0 < dist1) ? HandAim[0] : HandAim[1];
            Debug.Log(_hand.name);

            StartCoroutine(GrabInterpolateDT(InterpolationTime, _hitObj.transform.position,
                _hand.transform.position));
        }
        StackItem(_hitItem, Stack); //Stack the "fake" item in the reference on clawster's back
        Destroy(_hitObj);           //Delete item
    }
    
    IEnumerator GrabInterpolateDT(float dt, Vector3 start, Vector3 end) {
        Debug.Log("DOTween Coroutine started");
        
        Sequence seq = DOTween.Sequence();      //The whole coroutine process has been replaced by DOTween
        seq.Join(_hand.transform.DOPunchPosition(end, InterpolationTime - .1f,  8, 0.5f));
        seq.Join(_hand.transform.DOPunchRotation(end, InterpolationTime - .1f, 10, 0.7f));
        seq.Join(_hand.transform.DOPunchScale(   end, InterpolationTime - .1f,  4, 0.3f));
        seq.Play();
        for (dt = 0f; dt < InterpolationTime; dt += Time.deltaTime) yield return null; //Stalling behaviour
        
        IsGrabbing = false; //Maintains Clawster in position
    }

    private void StackItem(Item i, List<GameObject> stack) {
        stack.Add(i.StackedCounterpart);
    }
    
    
    //Deprecated function, do not uncomment unless necessary
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
}