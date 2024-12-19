using System;
using System.Collections;
using System.Collections.Generic;
using Local.Integration.Scripts.Game;
using UnityEngine;

public class Seagull : MonoBehaviour
{
    [SerializeField] private SeagullController _seagullController;

    private void OnTriggerEnter(Collider other) {
        _seagullController.PlayerPresence = true;
        _seagullController.gameObject.SetActive(true);
    }

    private void OnTriggerExit(Collider other) {
        _seagullController.PlayerPresence = false;
        _seagullController.gameObject.SetActive(false);
    }
}
