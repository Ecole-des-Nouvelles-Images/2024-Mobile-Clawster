using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

    public GameObject StackedCounterpart;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            GetComponent<Renderer>().sharedMaterial.SetVector("_OutlineColor", Vector4.one); //Set outline to white
        }
    }

    private void OnTriggerExit(Collider other) {
        GetComponent<Renderer>().material.SetVector("_OutlineColor", new Vector4(0, 0, 0, 1)); //Back to black
    }
}
