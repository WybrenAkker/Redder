using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirshipGun : MonoBehaviour
{
    public float health = 200;

    float fireDelay;
    public GameObject muzzleFlash;
    public AudioSource audioSource;

    public GameObject deathObject;

    public void Update()
    {
        if(health <= 0) //Check if health has dropped below 0.
        {
            Instantiate(deathObject, transform.position, Camera.main.transform.rotation); //Instantiate a death effect.
            Destroy(gameObject); //Destroy the gameObject.
        }
    }

    public void Execute(Transform target, AirshipAI.Turret thisTurret, LayerMask obstacleMask) //Called from the airship itself.
    {
        if(Vector2.Distance(transform.position, target.position) < thisTurret.range) //Check if the player is in range of the turret.
        {
            Vector2 targetDir = (target.position - transform.position).normalized; //Get direction from the turret to the player.
            if (Vector2.Angle(thisTurret.up, targetDir) < thisTurret.fov / 2 + 1) //Check if the player is inside the field of fiew of the turret.
            {
                thisTurret.barrel.up = targetDir; //Point the barrel towards the player.

                if(!Physics2D.Linecast(transform.position, target.position, obstacleMask)) //Check if there are no obstructions between the gun and the player.
                {
                    if(fireDelay <= 0) //Check if fireDelay has been reduced to 0. 
                    {
                        Fire(thisTurret); //Fire once.
                        fireDelay = thisTurret.fireRate; //Reset fireDelay.
                    }

                    fireDelay -= Time.deltaTime; //Remove realtime time from fireDelay.
                }
            }
        }
    }

    void Fire(AirshipAI.Turret thisTurret)
    {
        audioSource.clip = thisTurret.shootSound; //Assign the sound effect for firing.
        audioSource.Play(); //Play the sound effect.

        Instantiate(muzzleFlash, thisTurret.spawnPos.position, thisTurret.spawnPos.rotation); //Instantiate a muzzle flash effect.
        PoolManager.instance.ReuseObject(thisTurret.ammunition, thisTurret.spawnPos.position, new Quaternion(0,0,0,0), false); //Reuse a bullet from its corresponding pool.
    }

    public void Hit(float damage) //Called when the turret has been hit.
    {
        health -= damage;
    }
}
