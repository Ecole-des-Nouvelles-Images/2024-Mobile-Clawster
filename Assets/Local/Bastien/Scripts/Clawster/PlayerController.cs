using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    public Rigidbody RBC; 
    public PlayerInput playerInput;                 //Player input asset
    public float Speed;
    
    
    [SerializeField] private Camera _cam;           //Camera used to launch rays from
    [SerializeField] private Ray _grabRay;          //Said rays
    [SerializeField] private RaycastHit _grabRch;   //Class acting as callback when >W   ray hits
    
    private Vector2 _movement;
    private Vector3 _velocity;

    private GameObject _grabTarget;                 //Grabbed item
    
    public void OnJoystickMove(InputAction.CallbackContext context)
    {
        _movement = context.ReadValue<Vector2>();
    }

    // Update is called once per frame
    void Update()
    {
       Vector3 direction = new Vector3(_movement.x, 0f, _movement.y).normalized;

       if (direction.magnitude >= 0.1f && RBC.velocity.magnitude < 4)
       {
           float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + _cam.transform.eulerAngles.y;
           Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
           transform.rotation = Quaternion.LookRotation(direction, transform.up);
           RBC.AddForce(direction * (Speed * Time.deltaTime), ForceMode.Acceleration);
       }
       //_grabRay = _cam.ScreenPointToRay(Input.mousePosition);        //Send ray from camera
    }
}
