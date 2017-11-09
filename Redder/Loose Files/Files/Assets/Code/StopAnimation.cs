using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Small script that destroys animated sprites when they are finished playing. 
//The script also plays audio when available.
public class StopAnimation : MonoBehaviour
{
    public AudioSource soundEffect;
    private void Awake()
    {
        GetComponent<Animator>().SetTrigger("Play");//Plays the animation

        if (soundEffect != null) //When soundEffect is present, play that too.
        {
            soundEffect.Play();
        }
    }

    void Update ()
    {
		if(AnimatorIsPlaying() != true) //Check if animation has stopped playing each frame.
        {
            Destroy(gameObject); //Remove object for performance reasons.
        }
	}

    bool AnimatorIsPlaying()
    {
        return GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime < 1; //Check if the animator has finished playing the animation.
    }
}
