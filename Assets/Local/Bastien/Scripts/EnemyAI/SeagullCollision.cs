using System;
using System.Collections;
using System.Collections.Generic;
using Local.Integration.Scripts.Game;
using UnityEngine;

public class SeagullCollision : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    
    private void OnTriggerEnter(Collider other) {
        
        if(other.gameObject.CompareTag("Player") == false) return;
        _gameManager.GameOver();
    }
}
