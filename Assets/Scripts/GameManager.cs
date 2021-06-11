using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static Menus MenuHandler { get; private set; }

    /// <summary>Defines whether or not the game is paused.</summary>
    public bool Paused { get; private set; } = false;

    /// <summary>The current active scene.</summary>
    public Scene CurrentScene { get => SceneManager.GetActiveScene(); }

    [HideInInspector]
    public ObjectDestroyer m_ObjectDestroyer;

    private void Awake()
    {
        // Yikes... singleton patterns...
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            Debug.LogWarning("Instance of GameManager already exists. New instance destroyed.");
            return;
        }

        m_ObjectDestroyer = gameObject.AddComponent<ObjectDestroyer>();
        MenuHandler = (FindObjectOfType(typeof(Menus)) as Menus);
    }

    /// <summary>Pauses the game.</summary>
    public void TogglePause(bool input)
    {
        if (!input) return;

        // Invert pause status
        Paused = !Paused;

        Cursor.visible = Paused;
        Cursor.lockState = Paused ? CursorLockMode.None : CursorLockMode.Locked;

        // Toggle pause menu if one exists within the scene
        MenuHandler?.gameObject.SetActive(Paused);

        // Set time scale to simulate game pause
        Time.timeScale = Paused ? 0.0f : 1.0f;
    }

    /// <summary>Load scene using the scene's build index.</summary>
    /// <param name="sceneIndex">The scene's build index.</param>
    public void LoadScene(int sceneIndex)
    {
        // Ensure game is unpaused before reload
        if (Paused) TogglePause(true);

        SceneManager.LoadSceneAsync(sceneIndex);
    }

    /// <summary>Load scene using the name of the scene.</summary>
    /// <param name="sceneName">The name of the scene.</param>
    public void LoadScene(string sceneName)
    {
        // Ensure game is unpaused before reload
        if (Paused) TogglePause(true);

        SceneManager.LoadSceneAsync(sceneName);
    }

    /// <summary>Reloads the active scene.</summary>
    public void ReloadScene()
    {
        // Ensure game is unpaused before reload
        if (Paused) TogglePause(true);

        SceneManager.LoadSceneAsync(CurrentScene.buildIndex);
    }
}