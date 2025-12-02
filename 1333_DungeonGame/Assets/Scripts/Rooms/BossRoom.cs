using UnityEngine;

namespace Dungeon
{
    public class BossRoom : RoomBase
    {
        public EnemySpawner BossSpawner;
        private CombatController _combatController;

        void Awake()
        {
            if (BossSpawner == null)
            {
                BossSpawner = GetComponentInChildren<EnemySpawner>();
            }

            _combatController = FindFirstObjectByType<CombatController>();
        }

        public override void TriggerRoomEvent(Player player)
        {
            if (IsCleared) return;

            if (player == null)
            {
                Debug.LogError("BossRoom: TriggerRoomEvent called with null player.");
                return;
            }

            if (BossSpawner == null)
            {
                Debug.LogError("No BossSpawner assigned!");
                IsCleared = true;
                return;
            }

            Enemy boss = BossSpawner.Spawn(transform.position + Vector3.forward * 3f);

            if (boss != null)
            {
                boss.OnDeath.AddListener(OnBossDefeated);
                Debug.Log("Boss fight!");

                if (_combatController != null)
                {
                    _combatController.BeginCombat(player, boss);
                }
                else
                {
                    Debug.LogWarning("BossRoom: CombatController missing.");
                }
            }
            else
            {
                IsCleared = true;
            }
        }

        private void OnBossDefeated()
        {
            Debug.Log("Boss defeated!");
            IsCleared = true;

            if (GameManager.Exists())
            {
                GameManager.Instance.OnBossDefeated(null);
            }
        }
    }
}

