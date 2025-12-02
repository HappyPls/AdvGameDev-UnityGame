using UnityEngine;

namespace Dungeon
{
    public class EncounterRoom : RoomBase
    {
        [Header("Encounter Settings")]
        public EnemySpawner Spawner;

        private Enemy _spawnedEnemy;

        [Header("Combat")]
        public CombatController CombatController;

        void Awake()
        {
            if (Spawner == null)
            {
                Spawner = GetComponentInChildren<EnemySpawner>();
            }

            CombatController = FindFirstObjectByType<CombatController>();
            if (CombatController == null)
            {
                Debug.LogWarning("EncounterRoom: No CombatController found. Combat will not start.");
            }
        }

        public override void TriggerRoomEvent(Player player)
        {
            if (IsCleared)
            {
                Debug.Log("The room has already been cleared.");
                return;
            }

            //check for existing enemy. Dont spawn another if there is one.
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

            if (player == null)
            {
                Debug.LogError("EncounterRoom: TriggerRoomEvent called with null player.");
                return;
            }

            Vector3 center = transform.position;
            center.y += 1f;

            _spawnedEnemy = Spawner.Spawn(center);

            if (_spawnedEnemy != null)
            {
                _spawnedEnemy.OnDeath.AddListener(OnEnemyDefeated);
                Debug.Log("Encounter started in " + RoomName + "!");

                if (CombatController != null)
                {
                    CombatController.BeginCombat(player, _spawnedEnemy);
                }
                else
                {
                    Debug.LogWarning("EncounterRoom: CombatController missing, enemy will just stand in the room.");
                }
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
