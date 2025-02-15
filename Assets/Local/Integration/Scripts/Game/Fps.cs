using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Fps : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI fpsText; 
    private float deltaTime;

    void Update() {

        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsText.text = Mathf.Ceil(fps).ToString() + " FPS";
    }
}
