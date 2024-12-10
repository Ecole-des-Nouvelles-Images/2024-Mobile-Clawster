using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveObjectContainer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void DropChildren() {
        this.transform.DetachChildren();
        Debug.Log("I lost the kids!");
    }
}
