using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegAimGrounding : MonoBehaviour
{
   private GameObject _raycastOrigin;  // Origin of the ray
   int _layerMask;                      // LM to exclude unwanted colliders

   void Start()
   {
      // Only check for ground colliders
      _layerMask = LayerMask.GetMask("Ground");
      // Set the origin of the Raycast to the GameObject w/the script
      _raycastOrigin = transform.parent.gameObject;
   }

   void FixedUpdate()
   {
      RaycastHit hit;
      if (Physics.Raycast(_raycastOrigin.transform.position, -transform.up, out hit, Mathf.Infinity))
      {
         //Debug.Log("Ground Hit");
         transform.position = hit.point;
      }
   }
}
