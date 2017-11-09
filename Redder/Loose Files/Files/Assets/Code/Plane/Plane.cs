using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plane : MonoBehaviour {

    public float health = 100;
    GameObject manager;

    // Update is called once per frame
    private void Start()
    {
        manager = GameObject.FindGameObjectWithTag("WaveManager");
    }

    void Update ()
    {
		if(health <= 0)
        {
            manager.GetComponent<WaveManager>().activePlanes -= 1;
            health = 100;
            gameObject.SetActive(false);
        }
	}
}
