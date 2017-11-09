using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Same function as the "EndAnimation" script but for the player. This function is called when the player dies, and is capable of ending the game.
public class EndGame : MonoBehaviour
{
    public AudioSource soundEffect;
    private void Awake()
    {
        GetComponent<Animator>().SetTrigger("Play"); //Play the animation.
        if (soundEffect != null) //Check if the effect has a soundEffect.
        {
            soundEffect.Play(); //Play the soundEffect.
        }
    }

    void Update()
    {
        if (AnimatorIsPlaying() != true) //Check if the animation has stopped playing.
        {
            Destroy(gameObject); //Destroy the effect.
            GameObject.Find("Systems").GetComponent<IngameMenu>().OpenGameOverScreen(); //Open the game over screen.
        }
    }

    bool AnimatorIsPlaying()
    {
        return GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime < 1; //Return if the animator is still playing the animation.
    }
}
