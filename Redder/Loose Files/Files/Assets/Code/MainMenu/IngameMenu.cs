using UnityEngine;
using UnityEngine.SceneManagement;

public class IngameMenu : MonoBehaviour
{
    public bool inMenu;
    public GameObject background;
    public GameObject playButton;
    public GameObject exitButton;

    public GameObject _GOBackground;
    public GameObject _GOHeader;
    public GameObject _GORetryButton;
    public GameObject _GOExitButton;

    private void Update()
    {
        if(Input.GetButtonDown("Cancel")) //Check if the player presses the "Escape" button.
        {
            inMenu = !inMenu; //Inverses inMenu.
            if (inMenu) //If inMenu is true, open the ingame menu.
            {
                OpenMenu();
            }
            else //If inMenu is true, open the ingame menu.
            {
                CloseMenu();
            }
        }
    }

    public void CloseMenu() 
    {
        Time.timeScale = 1; //Set timescale to 1. (full)
        inMenu = false; //Set InMenu to false.

        //Set each UI element associated to the ingame menu to inactive.
        background.SetActive(false);
        playButton.SetActive(false);
        exitButton.SetActive(false);
    }

    void OpenMenu()
    {
        Time.timeScale = 0; //Set timescale to 0. 
        inMenu = true; //Set InMenu to true.

        //Set each UI element associated to the ingame menu to active.
        background.SetActive(true);
        playButton.SetActive(true);
        exitButton.SetActive(true);
    }

    public void OpenGameOverScreen()
    {
        //Set each UI element associated to the game over screen to active.
        _GOBackground.SetActive(true);
        _GORetryButton.SetActive(true);
        _GOExitButton.SetActive(true);
    }

    public void ReloadLevel()
    {
        Time.timeScale = 1; //sets timescale to 1. (Full)
        SceneManager.LoadScene(1); //Reloads the main scene.
    }
}
