using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace Dungeon
{
    public class GameOverUI : MonoBehaviour
    {
        [Header("Settings")]
        public string MainMenuSceneName = "MainMenu";
        public string GameplaySceneName = "GameStart";

        public GameObject GameOverPanel;

        private void Start()
        {
            if (GameOverPanel != null)
                GameOverPanel.SetActive(false);
        }

        public void ShowGameOver()
        {
            if (GameOverPanel != null)
                GameOverPanel.SetActive(true);
        }

        public void RestartGame()
        {
            if (!string.IsNullOrEmpty(GameplaySceneName))
            {
                SceneManager.LoadScene(GameplaySceneName);
            }
        }

        public void QuitToMainMenu()
        {
            if (!string.IsNullOrEmpty(MainMenuSceneName))
            {
                SceneManager.LoadScene(MainMenuSceneName);
            }
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
