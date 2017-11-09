﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuSystems : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene(1); //loads the main scene.
    }

    public void ExitGame() 
    {
        Application.Quit(); //Exits the application.
    }
}
