using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Small script that points an arrow to the ground, so the player knows what down is.
public class Arrowpointer : MonoBehaviour
{
	void Update ()
    {
        Vector3 dir = -(Vector3.zero - transform.parent.position).normalized; //Calculate direction from the player to the middle of the map. If you invert this direction you get the direction of gravity.

        transform.up = dir; //Face the arrow to the direction of gravity.
	}
}
