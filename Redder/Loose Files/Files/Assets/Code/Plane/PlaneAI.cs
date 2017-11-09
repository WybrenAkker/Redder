using UnityEngine;

public class PlaneAI : PoolObject
{
    public float health = 100;

    public float speed;
    public float rotationRate;
    Vector3 dir;
    public float minDistToTurn = 2f;
    public float range;
    public float fov;
    public float fireRate;

    public GameObject ammunition;

    float lifeTimer;
    bool visible = true;

    Transform target;
    public Rigidbody2D rigidBody;
    GameObject manager;
    GameObject parentAirship;

    public Transform leftGun, rightGun;

    public LayerMask obstacleMask;

    public GameObject muzzleFlash;
    public AudioSource audioSource;

    public GameObject deathObject;

    // Update is called once per frame
    private void Awake()
    {
        manager = GameObject.FindGameObjectWithTag("WaveManager");
    }

    float fireDelay;
    void Update()
    {
        Run();
    }

    void Run()
    {
        float distanceToPlayer = Vector2.Distance(target.position, transform.position); //Get the distance from the plane to the player.

        Vector3 toTarget = (target.position - transform.position).normalized; //Get the direction from the plane to the player.

        if (distanceToPlayer > minDistToTurn) //Check if the distance to the player is bigger than the minimal turning distance.
        {
            transform.up = Vector3.Lerp(transform.up, toTarget, rotationRate * Time.deltaTime); //Turn the plane towards the player.

            rigidBody.velocity = transform.up * speed; //Set the plane's velocity.
        }
        else
        {
            rigidBody.velocity = transform.up * speed; //Set the plane's velocity;
        }

        if (health <= 0) //Check if health has dropped below 0.
        {
            Instantiate(deathObject, transform.position, Camera.main.transform.rotation); //Instantiate a death effect. (explosion)
            manager.GetComponent<WaveManager>().activePlanes -= 1; //Reduce the active planes by one.
            if (parentAirship != null) //Check if the plane still has an airship.
            {
                parentAirship.GetComponent<AirshipAI>().activePlanes -= 1; //Reduce the airship's active planes by one.
            }
            health = 100; //Reset health.
            gameObject.SetActive(false); //Disable the plane.
        }

        if (distanceToPlayer < range) //Check if the player is within range of the plane.
        {
            if (Vector2.Angle(transform.up, toTarget) < fov / 2 + 1) //Check if the player is within the Field Of View of the plane.
            {
                if (!Physics2D.Linecast(transform.position, target.position, obstacleMask)) //Check if there is nothing in between the plane and the player.
                {
                    if (fireDelay <= 0) //Check if the fireDelay has decreased to or below 0
                    {
                        Fire(); //Fire once.
                        fireDelay = fireRate; //Reset the fireDelay.
                    }

                    fireDelay -= Time.deltaTime; //Remove realtime time from fireDelay.
                }
            }
        }
    }

    public void Hit(float damage) //Function that gets called whenever the plane is hit.
    {
        health -= damage;
    }

    void Fire()
    {
        audioSource.Play(); //Play a sound effect.

        Instantiate(muzzleFlash, leftGun.position, leftGun.rotation); //Instantiate a muzzle flash effect.
        PoolManager.instance.ReuseObject(ammunition, leftGun.position, Quaternion.identity, false); //Reuse a plane bullet to fire.
        Instantiate(muzzleFlash, rightGun.position, rightGun.rotation); //Instantiate a muzzle flash effect.
        PoolManager.instance.ReuseObject(ammunition, rightGun.position, Quaternion.identity, false); //Reuse a plane bullet to fire.
    }

    public override void OnObjectReuse(GameObject _ParentAirship) //Callback that gets called whenever the plane is reused.
    {
        parentAirship = _ParentAirship; //Set parent airship.
        gameObject.SetActive(true); //Set the plane to active.
        health = 100; //reset health.
        target = GameObject.FindGameObjectWithTag("Player").transform; //Set target to player.
        dir = (target.position - transform.position).normalized; //Get direction from plane to target.

        transform.up = -parentAirship.transform.right; //Set the initial rotation.
        rigidBody.velocity = transform.up * speed; //
    }
}
