using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dungeon
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("Scene Names")]
        [Tooltip("Name of the gameplay scene to load when clicking Play.")]
        public string GameSceneName = "DungeonScene";   // <-- CHANGE THIS to your game scene name

        public void PlayGame()
        {
            if (string.IsNullOrEmpty(GameSceneName))
            {
                Debug.LogError("MainMenuController: GameSceneName is not set!");
                return;
            }

            SceneManager.LoadScene(GameSceneName);
        }

        public void QuitGame()
        {
            Debug.Log("MainMenuController: QuitGame called.");

#if UNITY_EDITOR
            // Stop play mode when testing inside editor
            UnityEditor.EditorApplication.isPlaying = false;
#else
            // Quit the built game
            Application.Quit();
#endif
        }
    }
}
