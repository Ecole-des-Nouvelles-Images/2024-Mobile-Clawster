using System;
using UnityEngine;

public class Item : MonoBehaviour {
    public GameObject UICanvas;
    
    private void Update(){
        UICanvas.transform.rotation = Quaternion.Euler(0,0,0);
    }

    private void OnTriggerEnter(Collider other) {
        UICanvas.SetActive(true);
    }

    private void OnTriggerExit(Collider other) {
        UICanvas.SetActive(false);
    }
}
