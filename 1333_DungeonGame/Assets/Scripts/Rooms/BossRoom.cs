using UnityEngine;

namespace Dungeon
{
    public class BossRoom : RoomBase
    {
        public EnemySpawner BossSpawner;

        public override void TriggerRoomEvent(Player player)
        {
            if (IsCleared) return;
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

