using UnityEngine;

public class PlaneBullet : PoolObject
{
    public float speed;
    public Vector3 dir;
    public float minDamage, maxDamage;

    float timer;
    bool visible = true;

    public GameObject hitEffect;

    void Update()
    {
        Run();
    }

    void Run()
    {
        transform.Translate(dir * Time.deltaTime * speed); //Move the bullet.

        if (Physics2D.Raycast(transform.position, dir, 0.15f)) //Check if the bullet is about to hit something.
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 0.15f); //Get the object the bullet is about to hit.

            if (hit.transform.tag != "Enemy") //Check if the bullet hasn't hit an object marked "Enemy".
            {
                if (hit.transform.tag == "Player") //Check if the bullet is about to hit the player.
                {
                    hit.transform.SendMessageUpwards("Hit", Random.Range(minDamage, maxDamage)); //Send a message to the player that is has been hit. It's Hit() function will then be called.
                }
                gameObject.SetActive(false); //Deactivate the bullet.
                Instantiate(hitEffect, hit.point, transform.rotation); //Instantiate a hit effect.
            }
        }

        if (visible == false) //Check if the bullet isn't invisible (not being rendered).
        {
            timer += Time.deltaTime; //Count how long the bullet has been out of the camera's view.

            if (timer > 1.5f) //If the bullet has been invisible for 1.5 seconds, disable the bullet.
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            timer = 0; //If the bullet is visible (again), reset the invisibility timer.
        }
    }

    void OnBecameInvisible() //Callback that gets called when the bullet leaves the camera's view.
    {
        visible = false;
    }

    private void OnBecameVisible() //Callback that gets called when the bullet enters the camera's view.
    {
        visible = true;
    }

    public override void OnObjectReuse() //Callback that gets called when the object is reused by the pooling System.
    {
        timer = 0; //Reset the invisibility timer.
        dir = (GameObject.FindGameObjectWithTag("Player").transform.position - transform.position).normalized; //Asign the direction from the bullet's position to the player.
    }
}
