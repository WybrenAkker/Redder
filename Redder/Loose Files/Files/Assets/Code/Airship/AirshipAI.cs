using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirshipAI : MonoBehaviour
{
    public float health = 1000;

    [System.Serializable]
    public struct Turret
    {
        public Transform gun;
        public Transform barrel;
        public Transform spawnPos;
        public Vector2 up;

        public float fireRate;
        public GameObject ammunition;
        public AudioClip shootSound;

        public float range;
        public float fov;
    }

    [System.Serializable]
    public struct Hangar
    {
        public GameObject planeType;
        public int maxActivePlanes;
        public float timeBetweenPlaneSpawns;
        public Transform spawnPos;
    }

    public Transform player;
    GameObject manager;
    public LayerMask obstacleMask;

    public Turret[] turrets;

    public Hangar hangar;
    public int activePlanes;

    public GameObject deathObject;

    public void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        manager = GameObject.FindGameObjectWithTag("WaveManager");
    }

    void Update()
    {
        TrackTarget();
        SpawnPlanes();

        if(health <= 0) //Check if health has dropped below 0.
        {
            manager.GetComponent<WaveManager>().activeAirships -= 1; //Reduce the amount of active airships in game by one.
            Instantiate(deathObject, transform.position, Camera.main.transform.rotation); //Instantiate a death effect.
            Destroy(gameObject); //Destroy the airship.
        }
    }

    void TrackTarget()
    {
        foreach(Turret turret in turrets) //Cycle through all turrets on the airship, and execute their behaviours.
        {
            turret.gun.GetComponent<AirshipGun>().Execute(player, turret, obstacleMask);
        }
    }

    float spawnDelay;
    void SpawnPlanes()
    {
        if(activePlanes < hangar.maxActivePlanes) //Check if there are less active planes than maximally allowed.
        {
            if (spawnDelay <= 0) //Check if spawnDelay has been reduced to 0.
            {
                PoolManager.instance.ReuseObject(hangar.planeType, hangar.spawnPos.position, hangar.spawnPos.rotation, true, gameObject); //Reuse a plane from the planes pool.
                spawnDelay = hangar.timeBetweenPlaneSpawns; //Reset spawn delay.

                //Increase active planes by one.
                activePlanes += 1; 
                manager.GetComponent<WaveManager>().activePlanes += 1;
            }

            spawnDelay -= Time.deltaTime;//Remove realtime time from spawnDelay.
        }
    }

    public Vector2 DirFromAngle(float angleInDegrees, bool angleIsGlobal) //Calculates a direction from an angle.
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector2(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    private void OnDrawGizmosSelected() //Draws 2 lines visualising the FOV of every turret on the airship.
    {
        if (turrets.Length > 0)
        {
            foreach (Turret turret in turrets)
            {
                Vector2 gunPos;
                gunPos.x = turret.gun.position.x;
                gunPos.y = turret.gun.position.y;

                float angleAddition = Vector2.Angle(new Vector2(0,1), turret.up);
                Vector3 cross = Vector3.Cross(new Vector2(0, 1), turret.up);

                if (cross.z > 0)
                {
                    angleAddition = 360 - angleAddition;
                }

                Gizmos.DrawRay(gunPos, (DirFromAngle(angleAddition + turret.fov / 2, false) * turret.range));
                Gizmos.DrawRay(gunPos, (DirFromAngle(angleAddition - turret.fov / 2, false) * turret.range));
            }
        }
    }

    public void Hit(float damage) //Called when the airship is hit.
    {
        health -= damage;
    }
}
