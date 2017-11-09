using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//Small script that points an arrow to the nearest enemy.
public class EnemyDirection : MonoBehaviour
{
    Transform closestTarget;
    public Transform player;

	void Update ()
    {
        Run();
	}

    void Run()
    {
        if (closestTarget == null) //Check if the script currently has a target assigned.
        {
            List<GameObject> allEnemies = GameObject.FindGameObjectsWithTag("Enemy").ToList(); //Get all enemies from the game.

            foreach (GameObject enemy in allEnemies) //Cycle through all enemies.
            {
                if (enemy.activeSelf == false) //Check if the current cycle's gameobject is inactive.
                {
                    allEnemies.Remove(enemy); //Remove enemy if its inactive.
                }
            }

            allEnemies.Sort(delegate (GameObject c1, GameObject c2) //Sort all enemies from smallest distance to highest distance.
            {
                return Vector3.Distance(player.position, c1.transform.position).CompareTo
                            ((Vector3.Distance(player.position, c2.transform.position)));
            });

            closestTarget = allEnemies[0].transform; //Get the first enemy from the list. (This would be the closest one).
        }
        else
        {
            Vector3 dir = (closestTarget.position - transform.position).normalized; //Get direction from player to the target.
            dir.z = 0; //disregard the Z axis.

            transform.up = dir; //Turn arrow's up to the direction from player to the target.
        }

        if (closestTarget.gameObject.activeSelf == false) //If the target has been destroyed, reset closestTarget.
        {
            closestTarget = null;
        }
    }
}
