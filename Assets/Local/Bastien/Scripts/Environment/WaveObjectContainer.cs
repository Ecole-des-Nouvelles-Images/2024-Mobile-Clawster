using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveObjectContainer : MonoBehaviour
{

    [SerializeField] private GameObject _original;
    [SerializeField] private int _objectCount;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CreateObjects());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void DropChildren() {
        this.transform.DetachChildren();
        Debug.Log("I lost the kids!");
    }

    private IEnumerator CreateObjects()
    {
        for (int i = 0; i < _objectCount; i++)
        {
            Instantiate(_original, this.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(2f);
        }
        
    }
}
