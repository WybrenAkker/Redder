using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunTurret : MonoBehaviour
{
    float fireDelay;
    public GameObject muzzleFlash;
    public AudioSource audioSource;

    public void Execute(Transform target, GunTurretAI.Gun thisGun, Vector3 up, LayerMask obstacleMask)
    {
        if (Vector2.Distance(transform.position, target.position) < thisGun.range) //Check if the player is in range of the gun.
        {
            Vector3 targetDir = (target.position - transform.position).normalized; //Get direction from the gun to the player.

            if (Vector3.Angle(up, targetDir) < thisGun.fov / 2 + 1) //Check if the player is inside the gun's field of view.
            {
                thisGun.barrel.up = targetDir; //Rotate the barrel towards the player.

                if (!Physics2D.Linecast(transform.position, target.position, obstacleMask)) //Check if there is anything between the gun and the player.
                {
                    if (fireDelay <= 0) //Check if fireDelay has been reduced to 0.
                    {
                        Fire(thisGun); //Fire once.
                        fireDelay = thisGun.fireRate; //Reset fireDelay
                    }

                    fireDelay -= Time.deltaTime; //Remove realtime time from fireDelay.
                }
            }
        }
    }

    void Fire(GunTurretAI.Gun thisGun)
    {
        audioSource.clip = thisGun.shootSound; //Assign the sound effect to an "AudioSource" component.
        audioSource.Play(); //Play the sound effect.

        Instantiate(muzzleFlash, thisGun.spawnPos.position, thisGun.spawnPos.rotation); //Instantiate a muzzle flash.
        PoolManager.instance.ReuseObject(thisGun.ammunition, thisGun.spawnPos.position, Quaternion.identity, false); //Reuse a bullet from its pool.
    }
}
