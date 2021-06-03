using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    //<Summary> A handle to the main menu canvas so it can turned on/off when the player clicks a button.
    [SerializeField]
    private GameObject mainMenu;

    //<Summary> A handle to the setting canvas so it can turned on/off when the player clicks a button.
    [SerializeField]
    private GameObject settings;

    /*<Summary> Unity will attempt to find whatever scene that is passed in as a argument. Make sure that it is spelt correct and the scene is in the 
    build settings*/
    public void StartGame()
    {
        SceneManager.LoadScene("Level_001");
    }

    /*<Summary> Will turn the 'main menu' canvas off and turn the 'settings' canvas of so that the player can tweak the games settings.*/
    public void Settings()
    {
        mainMenu.SetActive(false); 
        settings.SetActive(true);
    }

    /*<Summary> Will turn the 'main menu' canvas on and turn the 'settings' canvas off so that the player can return from the settings once done.*/
    public void ReturnToMenu()
    {
        mainMenu.SetActive(true);
        settings.SetActive(false);
    }

    /*<Summary> Tells unity to shut down the game once the player has clicked onto the 'quit game' button.*/
    public void ExitGame()
    {
        Application.Quit();
    }
}
