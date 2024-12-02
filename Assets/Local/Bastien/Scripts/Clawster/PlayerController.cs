using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour {
    [SerializeField] private Rigidbody RBC;             //RigidBody entity to move Clawster 
    [SerializeField] private PlayerInput playerInput;   //Player input asset
    [SerializeField] private Camera PlayerCam;          //Camera used to launch rays from
    [SerializeField] private Collider _grabCollider;    //Collider used to grab objects

    public float Speed;         //Placed here for UI editor convenience
    public float SpeedCap;      //Maximum velocity, to avoid animation going out of hand
    public float SlowFactor;    //Factor by which Clawster will slow down if entering water
    
    //Several of the following variables are delcared here to optimize by caching
    private Vector2 _movement;  //normalized X and Z movement 
    private Vector3 _velocity;  //Normalized Vector3 conversion of _movement
    private float _targetAngle;
    
    private Ray _ray;               //Ray sent from camera
    private RaycastHit _hit;        //Hit information
    private GameObject _grabTarget; //Grabbed item
    
    
    private void Update() {
        if (Input.GetMouseButtonDown(0)) _ray = PlayerCam.ScreenPointToRay(Input.mousePosition);
            
        Vector3 direction = new Vector3(_movement.x, 0f, _movement.y).normalized;

        if (direction.magnitude >= 0.1f && RBC.velocity.magnitude < SpeedCap) {
            _targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            Vector3 moveDir = Quaternion.Euler(0f, _targetAngle, 0f) * Vector3.forward;
            transform.rotation = Quaternion.LookRotation(direction, transform.up);
            RBC.AddForce(direction * (Speed * Time.deltaTime), ForceMode.Force);
        }
    }   
    
    public void OnJoystickMove(InputAction.CallbackContext context) {
        _movement = context.ReadValue<Vector2>();
    }

    void OnGUI() {
        Debug.DrawRay(_ray.origin, _ray.direction * 1000, Color.yellow);
    }
}
