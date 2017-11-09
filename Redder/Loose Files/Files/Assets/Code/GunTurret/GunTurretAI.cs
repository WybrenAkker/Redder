using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunTurretAI : MonoBehaviour
{
    [System.Serializable]
    public struct Gun
    {
        public Transform barrel;
        public Transform spawnPos;
        public GameObject ammunition;
        public AudioClip shootSound;
        public float fireRate;

        public float fov;
        public float range;
    }

    public float health = 250;

    public Gun[] guns;

    Transform player;

    public LayerMask obstacleMask;

    public int gunTurretSpotIndex;
    GameObject manager;

    public Transform groundTarget;

    public float dropSpeed;

    public GameObject deathObject;

    public void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        manager = GameObject.FindGameObjectWithTag("WaveManager");
    }

    void Update()
    {
        if (transform.position == groundTarget.position) //Check if the gunTurret has reached the ground. (Spawns in the air, simulating some sort of airdrop)
        {
            TrackTarget(); 
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, groundTarget.position, Time.deltaTime * dropSpeed); //Move the gunTurret to the ground.
        }


        if(health <= 0) //Check if the turrets health has dropped below 0.
        {
            Instantiate(deathObject, transform.position, Camera.main.transform.rotation); //Instantiate a death effect.
            Destroy(gameObject); //Destroy the object.
            manager.GetComponent<WaveManager>().activeGunTurrets -= 1; //Reduce the active GunTurrets in the game by one.
            manager.GetComponent<WaveManager>().usedSpots -= 1; //Reduce the amount of used spots in the game by one.
            manager.GetComponent<WaveManager>().gunTurretSpawnPositions[gunTurretSpotIndex].gameObject.GetComponent<GTSpawnPosition>().isUsed = false; //Set the turret's spot's isused value to false.
        }
    }

    void TrackTarget()
    {
        foreach (Gun gun in guns) //Cycle through all guns on the gunturret. (Allows for multiple guns on one turret)
        {
            gun.barrel.GetComponent<GunTurret>().Execute(player, gun, groundTarget.up, obstacleMask); //Execute the function on the gun.
        }
    }

    public void Hit(float damage) //Function that is executed when the gun is hit.
    {
        health -= damage;
    }
}
