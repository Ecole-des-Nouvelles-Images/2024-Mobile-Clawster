using System.Collections.Generic;
using UnityEngine;

namespace Local.Integration.Scripts.Game
{
    public class ItemSpawner : MonoBehaviour
    {
        [Header("Spawner Settings")] [SerializeField]
        private Transform _spawnPoint; 
        [SerializeField] private List<GameObject> _itemPrefabs; 
        [SerializeField] private int _numberOfItemsToSpawn = 1; 

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I)) 
            {
                SpawnItems();
            }
        }

        private void SpawnItems()
        {
            for (int i = 0; i < _numberOfItemsToSpawn; i++)
            {
                int randomIndex = Random.Range(0, _itemPrefabs.Count);
                GameObject randomItem = _itemPrefabs[randomIndex];
                Instantiate(randomItem, _spawnPoint.position, _spawnPoint.rotation);
            }
        }
    }
}
