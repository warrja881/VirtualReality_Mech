using UnityEngine;

public class Menus : MonoBehaviour
{
    /*<Script Summary> This script handles all of the menus functionality that present in our game 'Bellum Apparatus'. If you want to use it just create any
     custom canvas, attach this script and than any function can be accessed through something like a button that is a child of the canvas. Need to make sure that
    you pass the canvas you want to work with into buttons handler.*/

    /// <summary>A handle to the main menu canvas so it can turned on/off when the player clicks a button.</summary>
    [SerializeField]
    private GameObject mainMenu = null;

    /// <summary>A handle to the setting canvas so it can turned on/off when the player clicks a button.</summary>
    [SerializeField]
    private GameObject settings = null;

    /// <summary>A handle to the main menu canvas so it can turned on/off when the player clicks a button.</summary>
    [SerializeField]
    private GameObject pauseMenu = null;

    private void Start()
    {
        if (pauseMenu != null) pauseMenu.SetActive(false);
    }

    /// <summary>Unity will attempt to find whatever scene that is passed in as a argument. Make sure that it is spelt correct and the scene is in the build settings.</summary>
    public void StartGame() => GameManager.Instance.LoadScene("Level_001");

    /// <summary>Will turn the 'main menu' canvas off and turn the 'settings' canvas of so that the player can tweak the games settings.</summary>
    public void Settings()
    {
        mainMenu.SetActive(false); 
        settings.SetActive(true);
    }

    /// <summary>Will turn the 'main menu' canvas on and turn the 'settings' canvas off so that the player can return from the settings once done.</summary>
    public void ReturnToMenu()
    {
        if (GameManager.Instance.CurrentScene.name == "MainMenu") {
            mainMenu.SetActive(true);
            settings.SetActive(false);
        }
        else
        {
            GameManager.Instance.LoadScene("MainMenu");
            mainMenu.SetActive(true);
            settings.SetActive(false);
        }
    }

    /// <summary>Tells unity to shut down the game once the player has clicked onto the 'quit game' button.</summary>
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

        Application.Quit();
    }

    /// <summary>Closes the pause menu.</summary>
    public void ClosePauseMenu()
    {
        if (GameManager.Instance.Paused)
            GameManager.Instance.TogglePause(true);
    }
}
