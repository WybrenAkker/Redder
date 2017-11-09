using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BriefingMenu : MonoBehaviour
{

    public GameObject briefingButton;

	void Start ()
    {
        Time.timeScale = 0; //Sets timescale to 0.
	}

    public void StartGame()
    {
        Time.timeScale = 1; //Sets timescale to 1. 
        //Sets all UI elements associated to the briefing to inactive.
        briefingButton.SetActive(false); 
        gameObject.SetActive(false);
    }
}
