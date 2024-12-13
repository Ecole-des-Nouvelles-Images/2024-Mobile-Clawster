using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Generic death box based on the collider's
 tag, useful for clearing a zone of all objects with tag*/
public class TagDeathBox : MonoBehaviour
{
    public string Tag;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(Tag) == true)
        {
            Destroy(other.gameObject);
        }
    }
}
