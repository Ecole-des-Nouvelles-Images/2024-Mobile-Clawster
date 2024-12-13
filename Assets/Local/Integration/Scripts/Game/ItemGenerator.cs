using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*This script is a direct fork of Noah's ItemSpawner,
 adding random position to the items along the parent.*/

public class ItemGenerator : MonoBehaviour {
    [Header("Spawner Settings")]
	[SerializeField] private Transform _origin;
    [SerializeField] private List<GameObject> _itemPrefabs;
    [SerializeField] private int _totalItems;
    [SerializeField] private float _minOffset, _maxOffset;
    
    private void Update() {
        if (Input.GetKeyDown(KeyCode.I)) {
            StartCoroutine(GenerateItems());
        }
    }

    /*private void SpawnItems() {
        for (int i = 0; i < _totalItems; i++) {
            int randomIndex = Random.Range(0, _itemPrefabs.Count);  //Select Random Index
            GameObject randomItem = _itemPrefabs[randomIndex];      //Select prefab from list using index
            Instantiate(randomItem, this.transform);                //Instantiates objects as children
        }
    }*/

    public IEnumerator GenerateItems() {
        for (int i = 0; i < _totalItems; i++) {
            float offx = Random.Range(_minOffset, _maxOffset);
			Vector3 itemPos = (_origin.position + new Vector3(offx, 0, 0));

			int yRot = Random.Range(0, 360);
			Quaternion itemRot = Quaternion.Euler(0, yRot, 0);

            int randomIndex = Random.Range(0, _itemPrefabs.Count);
            GameObject randomItem = _itemPrefabs[randomIndex];

            Instantiate(randomItem, itemPos, itemRot);
            yield return new WaitForSeconds(1f);
        }
    }
}