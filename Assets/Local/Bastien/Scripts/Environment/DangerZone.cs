using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerZone : MonoBehaviour {
    public GameObject EnemyController;

    private void OnTriggerEnter(Collider other) {
        
        if (!other.gameObject.CompareTag("Player")) return;

        Debug.Log("Player has entered");
        EnemyController.SetActive(true);
    }
    
    private void OnTriggerExit(Collider other) {
        
        if (!other.gameObject.CompareTag("Player")) return;
        
        EnemyController.SetActive(false);
    }
}
