using System;
using UnityEngine;

public class ItemGrab : MonoBehaviour
{
    [SerializeField] private Camera _cam;           //Camera used to launch rays from
    [SerializeField] private Ray _grabRay;          //Said rays
    [SerializeField] private RaycastHit _grabRch;   //Class acting as callback when ray hits
    public float GrabDistance;                      //Maximum distane between Clawster and the Item

    private GameObject _grabTarget;                 //Grabbed item
    
    void Update()
    {
        _grabRay = _cam.ScreenPointToRay(Input.mousePosition);        //Send ray from camera
    }

    private void OnTriggerStay(Collider other)
    {
        if (Input.GetMouseButtonDown(0) == false) return;
        
        if (Physics.Raycast(_grabRay, out _grabRch))
        {
            Debug.Log("Grabbed Item!");
            _grabTarget = _grabRch.collider.gameObject;
            GameObject.Destroy(_grabTarget.transform.parent.gameObject);
        }
        
    }
}
