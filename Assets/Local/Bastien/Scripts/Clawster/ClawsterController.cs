using UnityEngine;
using UnityEngine.InputSystem;

public class ClawsterController : MonoBehaviour
{
    public Rigidbody controller; 
    [SerializeField] private Camera _cam;       //Camera used to launch rays from
    public PlayerInput playerInput;             //Player input asset
    public CrabAnimationController cac;         //Reference to the main rig. Variable name is ass tho.
    public Collider grabCollider;               //Frustrum to grab objects 
    public float velocityCap;                   //Unused tor the moment
    
    public float speed = 600;
    private Vector2 _movement;
    private Vector3 _velocity;

    public float turnSmoothVelocity;
    public float turnSmoothTime = 0.1f;
    
    [SerializeField] private Ray _grabRay;          //Said rays
    [SerializeField] private RaycastHit _grabRch;   //Class acting as callback when >W   ray hits

    private GameObject _grabTarget;                 //Grabbed item
    
    public void OnJoystickMove(InputAction.CallbackContext context)
    {
        _movement = context.ReadValue<Vector2>();
    }

    // Update is called once per frame
    void Update()
    {
       Vector3 direction = new Vector3(_movement.x, 0f, _movement.y).normalized;

       if (direction.magnitude >= 0.1f && controller.velocity.magnitude < 4)
       {
           float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _cam.transform.eulerAngles.y;
           Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
           controller.AddForce(direction * speed * Time.deltaTime, ForceMode.Acceleration);
       }
       _grabRay = _cam.ScreenPointToRay(Input.mousePosition);        //Send ray from camera
    }
    
    private void OnTriggerStay(Collider other)
    {
        Debug.Log("OnTriggerStay");
        
        if (Input.GetMouseButtonDown(0) == false) return;
        
        if (Physics.Raycast(_grabRay, out _grabRch))
        {
            _grabTarget = _grabRch.collider.gameObject;
            
            if (!_grabTarget.CompareTag("Item")) return;
            
            _grabTarget = _grabRch.collider.gameObject;
            cac.GrabAndRemoveObject(_grabTarget);
            GameObject.Destroy(_grabTarget.transform.parent.gameObject);
        }
        
    }

    private void DeleteObject(GameObject obj)
    {
        GameObject.Destroy(obj);
    }
}
