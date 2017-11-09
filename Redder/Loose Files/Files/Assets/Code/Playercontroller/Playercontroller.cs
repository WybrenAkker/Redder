using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Playercontroller : MonoBehaviour
{
    public float health;
    float maxHealth;

    public bool fireMode;

    public Transform camera, turrets, thruster;
    public float camRotSpeed, camFollowSpeed, turretRotSpeed;
    Rigidbody2D rigidBody;
    public float groundCheckLength;
    public LayerMask groundMask;
    public float gravityMultiplier;

    public float speed;

    

    public AudioSource fireSound;
    public Transform leftGun, rightGun;

    public GameObject muzzleFlash;
    public GameObject ammunition;
    public float fireRate;
    float fireDelay;

    public GameObject deathObject;

    public float regenDelay;
    public float regenAddition;
    float regenTimer;

    public Image healthbar;


    void Start ()
    {
        rigidBody = GetComponent<Rigidbody2D>();
	}
	
	void Update ()
    {
        //Calls all functions that need to be updated each frame.
        RotateToMouse();
        CameraFollow();
        Gravity();
        PlayerInput();
        TrackHealth();
        RegenHealth();
    }

    void TrackHealth() //Tracks health and syncs the health variable to the visual health bar.
    {
        if(health <= 0)
        {
            Instantiate(deathObject, transform.position, Camera.main.transform.rotation);
            Destroy(gameObject);
        }

        healthbar.fillAmount = health / maxHealth;
    }

    void RegenHealth() //Regens health when player isn't using its thruster.
    {
        if (!fireMode) //Checks if the player isn't in firemode. (This would mean that the player could be using his thruster.
        {
            if (!Input.GetButton("Fire1") && !Input.GetButton("Jump")) //Check if player isn't using thruster
            {
                regenTimer += Time.deltaTime; //Add realtime time to timer.
            }
            else //Player is using thruster
            {
                regenTimer = 0; //Reset Timer.
            }
        }
        else //Player is in firemode. This would mean that the player is not using his thruster.
        {
            regenTimer += Time.deltaTime; //Add realtime time to timer.
        }

        if (regenTimer >= regenDelay) //Check if regentimer is larger than regenDelay.
        {
            if (health < maxHealth && health != maxHealth) //Check if health isn't at the max.
            {
                health += regenAddition * Time.deltaTime; //Add regenAddition to health.
            }
            else if (health != maxHealth) //Check if health is larger than maxHealth and isn't equal to maxHealth.
            {
                health = maxHealth; //Set health to maxHealth.
                regenTimer = 0; //reset regenTimer.
            }
        }
    }

    void CameraFollow() //Rotates and moves the camera to follow the player.
    {
        Vector3 dir = (Vector3.zero - transform.position).normalized; //Get direction from the camera to the middle of the map.
        dir.z = 0; //Removes the Z axis. (2D games don't have one).

        camera.up = Vector3.Slerp(camera.up, dir, camRotSpeed * Time.deltaTime); //Lerps the camera's up to dir. This way the ground is always on the bottom of the screen.

        Vector3 velocity = rigidBody.velocity;
        camera.position = Vector3.SmoothDamp(camera.position, new Vector3(transform.position.x, transform.position.y, camera.position.z), ref velocity, camFollowSpeed * Time.deltaTime); //Smoothly follows the player.
    }

    void RotateToMouse() //Rotates the player to face the mouse position.
    {
        Vector3 mousePos = Input.mousePosition; //Get mousePosition.

        mousePos = Camera.main.ScreenToWorldPoint(mousePos); //Convert mouse position (Screenspace) to worldSpace.

        mousePos.z = 0; //Set z axis to 0. 

        Vector3 dirToMouse = (mousePos - transform.position).normalized; //Gets direction from player to the mouse.

        if (Vector3.Distance(mousePos, transform.position) > 1f) //Checks if the distance between the player and the mouse isn't smaller than 1 unit. (This keeps the player from spinning out of control when the mouse is at its center.)
        {
            transform.up = Vector3.Lerp(transform.up, dirToMouse, turretRotSpeed * Time.deltaTime); //Lerp the player's up to the direction from player to mouse.
        }
    }

    void Gravity()
    {
        if (!Input.GetButton("Fire1") && fireMode || !Input.GetButton("Jump") && fireMode) //Check if the player isn't using his thruster.
        {
            Vector3 gravityDir = -(Vector3.zero - transform.position).normalized; //Get direction from the middle of the map to the player. The inverted direction is the direction of gravity.
 
            if (!Physics2D.Raycast(transform.position, gravityDir, groundCheckLength, groundMask))//Check if the player isn't already on the ground.
            {
                rigidBody.AddForce((gravityDir * 9.81f * gravityMultiplier) * Time.deltaTime); //Add a force to the player's rigidBody to simulate gravity.
            }
        }
    }

    void PlayerInput()
    {
        if(Input.GetButtonDown("Fire2")) //Check if player presses the right mouse button.
        {
            fireMode = !fireMode; //Swits between firemodes. (Firing & Flying)
        }

        if (!fireMode) //If not in fireMode, call the Move() function.
        {
            Move();
        }
        else //If in fireMode, call the Fire() function.
        {
            Attack();
        }
    }

    void Move() //Move the player
    {
        if(turrets.right != transform.up) //Check if the player's guns are rotated correctly.
        {
            //If not, turn the guns to face backwards.
            turrets.right = Vector3.Slerp(turrets.right, transform.up, turretRotSpeed * Time.deltaTime); 
            turrets.rotation = new Quaternion(0, 0, turrets.rotation.z, turrets.rotation.w);
        }

        if (Input.GetButton("Fire1") || Input.GetButton("Jump")) //Check if player is pressing the spacebar or the left mousebutton
        {
            rigidBody.velocity = Vector3.Lerp(rigidBody.velocity, transform.up * speed, 6 * Time.deltaTime); //Add a velocity to the player's rigidbody.
            thruster.gameObject.SetActive(true); //Set the thurster effect to active.
        }
        else
        {
            thruster.gameObject.SetActive(false); //Set the thurster effect to inactive.
        }
    }



    void Attack() //Fire bullets
    {
        thruster.gameObject.SetActive(false);
        if (turrets.right != -transform.up) //Check if the player's guns are rotated correctly.
        {
            //If not, turn the guns to face forwards.
            turrets.right = Vector3.Slerp(turrets.right, -transform.up, turretRotSpeed * Time.deltaTime);
            turrets.rotation = new Quaternion(0, 0, turrets.rotation.z, turrets.rotation.w);
        }
        if (Input.GetButton("Fire1") || Input.GetButton("Jump")) //Check if player is pressing the spacebar or the left mousebutton
        {
            if (fireDelay <= 0) //Check if the player's fireDelay has counted down to 0.
            {
                Fire(); //Fire 2 bullets.
                fireDelay = fireRate; //Set fireDelay to the fireRate set in the editor.
            }

            fireDelay -= Time.deltaTime; //Remove realtime time from fireDelay.
        }
    }

    void Fire()
    {
        fireSound.Play(); //Play an audio effect.

        Instantiate(muzzleFlash, leftGun.position, leftGun.rotation); //Instantiate a muzzleFlash.
        PoolManager.instance.ReuseObject(ammunition, leftGun.position, Quaternion.identity, false); //Reuse a bullet from the pool.
        Instantiate(muzzleFlash, rightGun.position, rightGun.rotation); //Instantiate a muzzleFlash.
        PoolManager.instance.ReuseObject(ammunition, rightGun.position, Quaternion.identity, false); //Reuse a bullet from the pool.
    }

    public void Hit(float damage) //Function that is called when the player is hit.
    {
        health -= damage; //Remove the overloaded damage from health.
        regenTimer = 0; //Reset the health regen timer.
    }

    public void ResetHealth(float _Health) //Reset health function. Called on start of each new wave.
    {
        health = _Health;
        maxHealth = _Health;
    }
}
