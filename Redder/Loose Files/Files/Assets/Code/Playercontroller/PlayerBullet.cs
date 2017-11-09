using UnityEngine;

//Behaviour for bullets shot by the player.
public class PlayerBullet : PoolObject
{
    public float speed;
    public Vector3 dir;

    public float minDamage, maxDamage;

    float timer;
    bool visible = true;

    public GameObject hitEffect;

    // Update is called once per frame
    void Update()
    {
        Run();
    }

    void Run()
    {
        transform.Translate(dir * Time.deltaTime * speed); //Move the bullet.


        if (Physics2D.Raycast(transform.position, dir, 0.15f)) //Check if the bullet is about to hit something.
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 0.15f); //Get the object the raycast has hit.

            if (hit.transform.tag != "Player") //Check if the raycast hasn't hit the player.
            {
                if (hit.transform.tag == "Enemy") //Check if the raycast has hit an enemy.
                {
                    hit.transform.SendMessageUpwards("Hit", Random.Range(minDamage, maxDamage)); //Send a message to the enemy that is has been hit. its Hit() function will be called.
                }
                gameObject.SetActive(false); //Set the bullet to inactive.
                Instantiate(hitEffect, hit.point, transform.rotation); //Instantiate the bullet's hit effect.
            }
        }

        if (visible == false) //Check if the bullet is invisible (not being rendered, outside of the camera's view).
        {
            timer += Time.deltaTime; //Count the time the bullet has been invisible.

            if (timer > 1.5f) //If the bullet has been invisible for 1.5 seconds, disable the bullet.
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            timer = 0; //Reset invisibility timer when bullet has become visible again.
        }
    }

    void OnBecameInvisible() //Callback when object has left the camera view.
    {
        visible = false;
    }

    private void OnBecameVisible() //Callback when object has entered the camera view.
    {
        visible = true;
    }

    public override void OnObjectReuse() //Callback when the object has been reused.
    {
        timer = 0; //reset the invisibility timer.
        Vector3 mousePos = Input.mousePosition; //Get the mousePosition

        mousePos = Camera.main.ScreenToWorldPoint(mousePos); //Convert the mouse Position from screenspace to worldspace.

        mousePos.z = 0; //disregard the Z axis.

        Vector3 dirToMouse = (mousePos - transform.position).normalized; //Get direction from bullet to mouse.
        dir = dirToMouse; //Assign move direction of bullet.
    }
}
