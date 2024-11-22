using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

/*This code was copied straight from the video just to get the script working
 * We need to rework it ASAP to adapt it to a touch screen, and remove some
 * of the horrid tecnhiques used to make movement possible*/
public class Controller : MonoBehaviour
{
    public Rigidbody controller; 
    public Transform camTransform;
    public PlayerInput playerInput;
    public float velocityCap;
    
    public float speed = 6;
    private Vector2 _movement;
    private Vector3 _velocity;

    public float turnSmoothVelocity;
    public float turnSmoothTime = 0.1f;

    public void InputPlayer(InputAction.CallbackContext context)
    {
        _movement = context.ReadValue<Vector2>();
    }

    // Update is called once per frame
    void Update()
    {
        
       Vector3 direction = new Vector3(_movement.x, 0f, _movement.y).normalized;

       if (direction.magnitude >= 0.1f && controller.velocity.magnitude < 4)
       {
           float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + camTransform.eulerAngles.y;
           
           Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
           controller.AddForce(direction * speed * Time.deltaTime, ForceMode.Acceleration);
       }
    }
}
