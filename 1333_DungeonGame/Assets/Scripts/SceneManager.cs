using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System.Collections;
public class SceneSwitcher : MonoBehaviour
{
    public string sceneToLoad; // Assign the name of the target scene in the Inspector
    public string sceneToUnload;

    void OnTriggerEnter2D(Collider2D other) // Use OnTriggerEnter for 3D colliders
    {
        // Check if the colliding object is the player (e.g., by tag)
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive);
            SceneManager.UnloadSceneAsync(sceneToUnload);
        }
    }
    public void LoadGame()
    {
        SceneManager.LoadScene("MainScene", LoadSceneMode.Single);

        // Load your actual game scenes
        //SceneManager.LoadScene("MainScene", LoadSceneMode.Additive);
        SceneManager.LoadScene("GameStart", LoadSceneMode.Additive);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");

        Application.Quit();
    }
}