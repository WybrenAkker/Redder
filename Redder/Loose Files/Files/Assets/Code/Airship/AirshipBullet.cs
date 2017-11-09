using UnityEngine;

public class AirshipBullet : PoolObject
{
    public float speed;
    public Vector3 dir;
    public float minDamage, maxDamage;

    float timer;
    bool visible = true;

    public GameObject hitEffect;

    // Update is called once per frame
    void Update ()
    {
        transform.Translate(dir * Time.deltaTime * speed); //Move the bullet.


        if(Physics2D.Raycast(transform.position, dir, 0.15f)) //Check if the bullet is about to hit something.
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 0.15f); //Get the object the bullet is about to hit.

            if (hit.transform.tag != "Enemy") //Check if the bullet isn't about to hit an enemy. 
            {
                if (hit.transform.tag == "Player") //Check if the bullet is about to hit the player.
                {
                    hit.transform.SendMessageUpwards("Hit", Random.Range(minDamage, maxDamage)); //Send a message to the player notifying it that it has been hit.
                }
                gameObject.SetActive(false); //Disable the bullet.
                Instantiate(hitEffect, hit.point, transform.rotation); //Instantiate a hit effect.
            }
        }

        if(visible == false) //Check if the bullet has left the screen.
        {
            timer += Time.deltaTime; //Count the time the bullet has been outside of the camera view.

            if(timer > 1.5f) //If the bullet has been outside of the camera view for more than 1.5 seconds, disable the bullet.
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            timer = 0; //If the object has (re)entered the screen, reset the timer.
        }
	}

    

    void OnBecameInvisible() //Called when the bullet leaves the screen.
    {
        visible = false;
    }

    private void OnBecameVisible() //Called when the bullet enters the screen.
    {
        visible = true;
    }

    public override void OnObjectReuse() //Called when the object is reused.
    {
        timer = 0; //Reset the timer.
        dir = (GameObject.FindGameObjectWithTag("Player").transform.position - transform.position).normalized; //Get the direction from the bullet to the player.

    }
}
