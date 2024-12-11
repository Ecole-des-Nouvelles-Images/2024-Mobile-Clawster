using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LineRaycast : MonoBehaviour {
    [SerializeField] private List<GameObject> _splinePoints;
    [SerializeField] private float _yMargin;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject curvePoint in _splinePoints) {
            
            Vector3 castPosition = new Vector3( curvePoint.transform.position.x,
                                                transform.position.y, 
                                                curvePoint.transform.position.z);
            
            if (Physics.Raycast(castPosition, Vector3.down, out RaycastHit hit, Mathf.Infinity)) {
                
                if(hit.collider.CompareTag("Floor") == false) return;
                
                curvePoint.transform.position = new Vector3(
                    curvePoint.transform.position.x, hit.point.y + _yMargin, curvePoint.transform.position.z);
            }
        }
        
    }
    
}
