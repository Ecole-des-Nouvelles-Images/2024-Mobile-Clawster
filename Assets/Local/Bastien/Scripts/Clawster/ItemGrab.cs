using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ItemGrab : MonoBehaviour {
    [Header("Essential Settings")]
    public GameObject[] HandAim;    //Hands with index 0 and 1
    public Collider GrabCollider;   //Actual collision of the grab zone
    public Camera PlayerCam;        //Main game camera
    public Button GrabButton;       //UI Button to grab
    
    [Header("Stacked Objects (Not Implemented)")]
    public List<GameObject> Stack;  //Stack that Clawster has on his back
    
    [Header("Animation")]
    public Animator HandAnimator;
    public float InterpolationTime; //
    public bool IsGrabbing { get; private set; }
    [SerializeField] private float _despawnTime;
    
    private GameObject _hand;   //Soon to be selected hand
    private GameObject _hitObj; //Object hit by raycast
    private Item _hitItem;      //Item class belonging to the prefab

    private void Start() {
        HandAnimator = GetComponentInParent<Animator>();
    }

    private void Update() {     //Useless LMAO
    }

    private void OnTriggerEnter(Collider other) { 
        GrabButton.gameObject.SetActive(true);
        _hitObj = other.gameObject;                 //Get the object that is parent of the collider
        _hitItem = _hitObj.GetComponent<Item>();    //Get the stacked version of the item
        _hitItem.GetComponent<Renderer>().sharedMaterial.SetVector("_OutlineColor", Vector4.one);
    }

    private void OnTriggerExit(Collider other) {
        GrabButton.gameObject.SetActive(false);
        _hitObj = null;
        _hitItem.GetComponent<Renderer>().material.SetVector("_OutlineColor", new Vector4(0,0,0,1));
    }

    public void Grab() {
        if (_hitObj.CompareTag("Item")) {   //Raycast part is gone, as we now use the UI to grab
            IsGrabbing = true;
            HandAnimator.SetBool("IsGrabbing", true);

            float dist0 = Vector3.Distance(_hitObj.transform.position, HandAim[0].transform.position);
            float dist1 = Vector3.Distance(_hitObj.transform.position, HandAim[1].transform.position);

            _hand = (dist0 < dist1) ? HandAim[0] : HandAim[1];
            Debug.Log(_hand.name);
            
            StartCoroutine(GrabAnimate(InterpolationTime));
        }
        StackItem(_hitItem, Stack);     //Stack the "fake" item in the reference on clawster's back
        Destroy(_hitObj, _despawnTime); //Delete item
        IsGrabbing = false;
    }
    
    IEnumerator GrabAnimate(float dt) {
        for (dt = 0f; dt < InterpolationTime; dt += Time.deltaTime) yield return null; //Stalling behaviour
        HandAnimator.SetBool("IsGrabbing", false);
        IsGrabbing = false; //Maintains Clawster in position
    }

    private void StackItem(Item i, List<GameObject> stack) {
        stack.Add(i.StackedCounterpart);
        
    } 
    
    //Deprecated function, do not uncomment unless necessary
    /*IEnumerator GrabInterpolateDT(float dt, Vector3 start, Vector3 end) {
        Debug.Log("DOTween Coroutine started");
        
        Sequence seq = DOTween.Sequence();      //The whole coroutine process has been replaced by DOTween
        seq.Join(_hand.transform.DOPunchPosition(end, InterpolationTime - .1f,  0, 0f));
        seq.Join(_hand.transform.DOPunchRotation(end, InterpolationTime - .1f, 0, 0f));
        seq.Join(_hand.transform.DOPunchScale(   end, InterpolationTime - .1f,  0, 0f));
        seq.Play();
        for (dt = 0f; dt < InterpolationTime; dt += Time.deltaTime) yield return null; //Stalling behaviour
        
        IsGrabbing = false; //Maintains Clawster in position
    }*/
    
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