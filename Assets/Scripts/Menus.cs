using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menus : MonoBehaviour
{
    /*<Script Summary> This script handles all of the menus functionality that present in our game 'Bellum Apparatus'. If you want to use it just create any
     custom canvas, attach this script and than any function can be accessed through something like a button that is a child of the canvas. Need to make sure that
    you pass the canvas you want to work with into buttons handler.*/

    /*<Varibale Summary> A handle to the main menu canvas so it can turned on/off when the player clicks a button.*/
    [SerializeField]
    private GameObject mainMenu;

    /*<Vairbale Summary> A handle to the setting canvas so it can turned on/off when the player clicks a button.*/
    [SerializeField]
    private GameObject settings;

    /*<Varibale Summary> A handle to the main menu canvas so it can turned on/off when the player clicks a button.*/
    [SerializeField]
    private GameObject pauseMenu;

    private Scene currentScene;
    private string currSceneName;

    void Awake()
    {
        currentScene = SceneManager.GetActiveScene();
        currSceneName = currentScene.name;
    }

    /*<Function Summary> Unity will attempt to find whatever scene that is passed in as a argument. Make sure that it is spelt correct and the scene is in the 
    build settings*/
    public void StartGame()
    {
        SceneManager.LoadScene("Level_001");
    }

    /*<FunctionSummary> Will turn the 'main menu' canvas off and turn the 'settings' canvas of so that the player can tweak the games settings.*/
    public void Settings()
    {
        mainMenu.SetActive(false); 
        settings.SetActive(true);
    }

    /*<Function Summary> Will turn the 'main menu' canvas on and turn the 'settings' canvas off so that the player can return from the settings once done.*/
    public void ReturnToMenu()
    {
        if(currSceneName == "MainMenu") {
            mainMenu.SetActive(true);
            settings.SetActive(false);
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
            mainMenu.SetActive(true);
            settings.SetActive(false);
            Time.timeScale = 1.0f;
        }
    }

    /*<Funciton Summary> Tells unity to shut down the game once the player has clicked onto the 'quit game' button.*/
    public void ExitGame()
    {
        Application.Quit();
    }

    /*<Function Summary> Allows for the player to press the set up 'pause' input to bring up the pause menu and freeze the game.*/
    public void PauseGame()
    {
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
    }

    /*<Function Summary> Allows for the player to resume the game once they are finished what it was they were doing.*/
    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1.0f;
    }
}
