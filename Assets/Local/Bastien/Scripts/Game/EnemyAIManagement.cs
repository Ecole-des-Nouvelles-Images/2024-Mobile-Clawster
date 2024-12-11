using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIManagement : MonoBehaviour {
    public GameObject Enemy;

    public float PeaceTime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (PeaceTime > 0f ) {
            PeaceTime -= Time.deltaTime;

            if (PeaceTime <= 0f) {
                PeaceTime = 0f;
                Enemy.SetActive(true);
            }
        }
    }
}
