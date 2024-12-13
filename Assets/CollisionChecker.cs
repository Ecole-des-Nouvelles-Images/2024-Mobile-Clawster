using System.Collections.Generic;
using UnityEngine;

public class CollisionChecker : MonoBehaviour {
    
    public GameObject CollisionObject;
    
    [SerializeField] private List<string> CollisionLayers;
    
    public bool IsColliding => CollisionObject != null;
    
    private void OnTriggerStay(Collider other) {
        if (!CollisionLayers.Contains(other.tag)) return;
        CollisionObject = other.gameObject;
    }
}
