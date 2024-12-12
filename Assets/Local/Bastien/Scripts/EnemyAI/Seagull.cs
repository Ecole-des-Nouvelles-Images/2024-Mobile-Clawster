using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seagull : MonoBehaviour
{
    [SerializeField] private SeagullController _seagullController;

    private void OnTriggerEnter(Collider other) {
        _seagullController.PlayerPresence = true;
        _seagullController.Reset();
        _seagullController.DoCycle();
    }

    private void OnTriggerExit(Collider other) {
        _seagullController.PlayerPresence = false;
    }
}
