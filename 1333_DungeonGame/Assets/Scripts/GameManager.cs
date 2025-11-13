using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dungeon
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [Header("Boss Routing")]
        public EnemyData FinalBossData;

        [Header("State")]
        public bool GameEnded = false;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public static bool Exists()
        {
            return Instance != null;
        }

        public bool IsFinalBoss(Enemy enemy)
        {
            if (enemy == null) return false;
            if (enemy.Data == null) return false;
            if (FinalBossData == null) return false;
            return enemy.Data == FinalBossData;
        }

        public void OnBossDefeated(Enemy boss)
        {
            if (GameEnded) return;

            Debug.Log("Final Boss defeated: " + (boss != null ? boss.DisplayName : "Unknown"));
            GameOver(true);
        }

        public void GameOver(bool playerWon)
        {
            if (GameEnded) return;
            GameEnded = true;

            if (playerWon)
            {
                Debug.Log("GAME OVER: You Win!");
            }
            else
            {
                Debug.Log("GAME OVER: You Lose!");
            }

            //Add: Show UI, stop input, load result screen, allow restart
        }

        //Restart Helper
        public void RestartScene()
        {
            GameEnded = false;
            Time.timeScale = 1f;
            Scene current = SceneManager.GetActiveScene();
            SceneManager.LoadScene(current.buildIndex);
        }
        public void OnDungeonReady(DungeonController controller)
        {
            Debug.Log("Dungeon ready: GameManager linked.");
        }

        public void OnPlayerDied(Player player)
        {
            if (GameEnded) return;
            Debug.Log("Player died.");
            GameOver(false);
        }
    }
}