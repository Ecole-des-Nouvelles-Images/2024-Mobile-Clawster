using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*This code was copied straight from the video just to get the script working
 * We need to rework it ASAP to adapt it to a touch screen, and remove some
 * of the horrid tecnhiques used to make movement possible*/
public class Controller : MonoBehaviour
{
    public Rigidbody controller;
    public Transform camera;
    public float velocityCap;
    
    public float speed = 6;
    private Vector3 _velocity;
    

    public float turnSmoothVelocity;
    public float turnSmoothTime = 0.1f;
    
    // Update is called once per frame
    void Update()
    {
       float horizontal = Input.GetAxis("Horizontal");
       float vertical = Input.GetAxis("Vertical");
       Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

       if (direction.magnitude >= 0.1f && controller.velocity.magnitude < 4)
       {
           float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + camera.eulerAngles.y;
           
           Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
           controller.AddForce(moveDir.normalized * speed * Time.deltaTime, ForceMode.Acceleration);
       }
    }
}
