using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeagullCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        
        if(other.gameObject.CompareTag("Player") == false) return;
        
        Debug.Log("Clawster Hit");
    }
}
