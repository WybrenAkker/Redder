using UnityEngine;

public class MissileAI : PoolObject
{
    public float health = 10;

    public float speed;
    public float rotationRate;
    Vector3 dir;
    public float lifeTime;
    public float minDamage;
    public float maxDamage;

    float lifeTimer;
    bool visible = true;

    Transform target;
    public Rigidbody2D rigidBody;
    GameObject manager;

    public GameObject explosion;
    // Update is called once per frame
    private void Awake()
    {
        manager = GameObject.FindGameObjectWithTag("WaveManager");
    }

    void Update()
    {
        lifeTimer += Time.deltaTime; //Count the time the missile has been active.

        if(lifeTimer > lifeTime || health == 0) //Check if the missile's health has dropped below zero or has been alive for too long.
        {
            gameObject.SetActive(false); //Disable the gameObject.
            Instantiate(explosion, transform.position, transform.rotation); //Instantiate an explosion effect.
        }

        Vector3 toTarget = target.position - transform.position; //Get direction from the missile to the player.
        Vector3 newVelocity = Vector3.RotateTowards(rigidBody.velocity, toTarget, rotationRate * Mathf.Deg2Rad * Time.fixedDeltaTime, 0); //Calculate the direction to the player.
        newVelocity.z = 0; //Disregard the Z axis.

        rigidBody.velocity = newVelocity; //Assign the new direction as velocity.
        transform.up = newVelocity; //Set the transform.up to the new direction.

        if (Physics2D.Raycast(transform.position, transform.up, 0.15f)) //Check if the missile is about to hit something.
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, 0.15f); //Get the object the missile is about to hit.

            if (hit.transform.tag != "Enemy") //Check if the missile isn't about to hit an enemy.
            {
                if (hit.transform.tag == "Player") //Check if the player is about to hit the player.
                {
                    hit.transform.SendMessageUpwards("Hit", Random.Range(minDamage, maxDamage)); //Send a message to the player saying it has been hit. Hit() will then be called.
                }
                gameObject.SetActive(false); //Disable the missile.
                Instantiate(explosion, hit.point, transform.rotation); //Instantiate an explosion effect.
            }
        }
    }

    public override void OnObjectReuse() //Callback that gets called when the missile is reused.
    {
        target = GameObject.FindGameObjectWithTag("Player").transform; //Assign player as target.
        dir = (target.position - transform.position).normalized; //Get direction from the missile to the player.

        transform.up = dir; //Set the missile's up to the direction to the player.
        rigidBody.velocity = transform.up * speed; //Set the inital velocity.
        lifeTimer = 0; //reset lifeTimer.
        health = 10; //reset health.
    }

    public void Hit(float damage) //Function that gets called whenever the missile is hit.
    {
        health -= damage;
    }
}
