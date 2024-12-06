using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour {
    [SerializeField] private Rigidbody controller;
    [SerializeField] private PlayerInput playerInput; //Player input asset
    [SerializeField] private Camera PlayerCam; //Camera used to launch rays from
    [SerializeField] private ItemGrab ItemGrab; //Grab animation control

    [SerializeField] public GameManager gm;

    public float Speed; //Placed here for UI editor convenience
    public float SpeedCap; //Maximum velocity, to avoid animation going out of hand
    public float SlowFactor; //Factor by which Clawster will slow down if entering water

    //Several of the following variables are declared here to optimize by caching
    private Vector2 _movement; //normalized X and Z movement 
    private Vector3 _velocity; //Normalized Vector3 conversion of _movement
    private float _targetAngle;

    private Ray _ray; //Ray sent from camera
    private RaycastHit _hit; //Hit information
    private GameObject _grabTarget; //Grabbed item
    
    private void Update() {
        if (!gm.GameStarted) return;

        Vector3 direction = new Vector3(_movement.x, 0f, _movement.y).normalized;

        if ((direction.magnitude >= 0.1f && controller.velocity.magnitude < SpeedCap) && (!ItemGrab.IsGrabbing)) {
            _targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            Vector3 moveDir = (Quaternion.Euler(1f, _targetAngle, 1f) * Vector3.forward).normalized;
            transform.rotation = Quaternion.LookRotation(direction, transform.up);
            controller.AddForce(moveDir * ((Speed / SlowFactor + 1) * Time.deltaTime), ForceMode.Force);
        }
    }


    public void OnJoystickMove(InputAction.CallbackContext context) {
        _movement = context.ReadValue<Vector2>();
    }

    void OnGUI() {
        Debug.DrawRay(_ray.origin, _ray.direction * 1000, Color.yellow);
    }
}