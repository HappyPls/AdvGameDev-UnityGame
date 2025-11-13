using UnityEngine;

namespace Dungeon
{
    public class EncounterRoom : RoomBase
    {
        [Header("Encounter Settings")]
        public EnemySpawner Spawner;

        private Enemy _spawnedEnemy;

        void Awake()
        {
            if (Spawner == null)
            {
                Spawner = GetComponentInChildren<EnemySpawner>();
            }
        }

        public override void TriggerRoomEvent(Player player)
        {
            // If room is cleared, do nothing forever.
            if (IsCleared)
            {
                Debug.Log("The room has already been cleared.");
                return;
            }

            // If an enemy is already spawned and still alive, don't spawn another one.
            if (_spawnedEnemy != null && _spawnedEnemy.IsAlive)
            {
                Debug.Log("EncounterRoom: Enemy already present in " + RoomName + ", not spawning a new one.");
                return;
            }

            if (Spawner == null)
            {
                Debug.LogError("EncounterRoom has no Spawner!");
                IsCleared = true;
                return;
            }

            Vector3 center = transform.position;
            center.y += 1f;

            _spawnedEnemy = Spawner.Spawn(center);

            if (_spawnedEnemy != null)
            {
                _spawnedEnemy.OnDeath.AddListener(OnEnemyDefeated);
                Debug.Log("Encounter started in " + RoomName + "!");
            }
            else
            {
                Debug.LogWarning("EncounterRoom: Spawner failed to spawn an enemy.");
                IsCleared = true;
            }
        }
        private void OnEnemyDefeated()
        {
            Debug.Log("Enemy in " + RoomName + " has been defeated!");
            IsCleared = true;

            if (_spawnedEnemy != null)
            {
                _spawnedEnemy.OnDeath.RemoveListener(OnEnemyDefeated);
                _spawnedEnemy = null;
            }
        }
    }
}
