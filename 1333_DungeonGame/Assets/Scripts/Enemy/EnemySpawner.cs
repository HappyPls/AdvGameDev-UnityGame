using UnityEngine;

namespace Dungeon
{
    [System.Serializable]
    public class SpawnEntry
    {
        [Tooltip("Enemy prefab with an Enemy component + EnemyData already assigned.")]
        public Enemy EnemyPrefab;

        [Min(0f)]
        public float Weight = 1f;
    }

    public class EnemySpawner : MonoBehaviour
    {
        [Header("Spawn Setup")]
        [Tooltip("Weighted list of enemy prefabs to pick from")]
        public SpawnEntry[] Options;

        [Header("Debug / Info")]
        public Enemy LastSpawnedEnemy;
        public SpawnEntry LastPickedEntry;
        public bool LogPicks = false;

        void OnValidate()
        {
            if (Options == null) return;

            int i = 0;
            while (i < Options.Length)
            {
                if (Options[i] != null && Options[i].Weight < 0f)
                {
                    Options[i].Weight = 0f;
                }
                i += 1;
            }
        }

        public Enemy Spawn(Vector3 position)
        {
            SpawnEntry pick = PickWeightedEntry();
            if (pick == null || pick.EnemyPrefab == null)
            {
                Debug.LogError("EnemySpawner: No valid Enemy prefab could be selected (check Options and weights).");
                return null;
            }

            Enemy e = Instantiate(pick.EnemyPrefab, position, Quaternion.identity);
            LastSpawnedEnemy = e;
            LastPickedEntry = pick;

            if (LogPicks)
            {
                EnemyData data = e.Data;
                string namePart = data != null ? data.DisplayName : e.name;
                bool isBoss = data != null && data.IsBoss;
                Debug.Log("EnemySpawner picked: " + namePart + (isBoss ? " [BOSS]" : ""));
            }

            return e;
        }

        public Enemy SpawnHere()
        {
            return Spawn(transform.position);
        }

        private SpawnEntry PickWeightedEntry()
        {
            if (Options == null || Options.Length == 0) return null;

            float total = 0f;
            int i = 0;
            while (i < Options.Length)
            {
                SpawnEntry entry = Options[i];
                if (entry != null && entry.EnemyPrefab != null && entry.Weight > 0f)
                {
                    total += entry.Weight;
                }
                i += 1;
            }

            if (total <= 0f) return FallbackFirstValid();

            float r = Random.value * total;
            i = 0;
            while (i < Options.Length)
            {
                SpawnEntry entry = Options[i];
                if (entry != null && entry.EnemyPrefab != null && entry.Weight > 0f)
                {
                    if (r < entry.Weight)
                    {
                        return entry;
                    }
                    r -= entry.Weight;
                }
                i += 1;
            }

            return FallbackFirstValid();
        }

        private SpawnEntry FallbackFirstValid()
        {
            if (Options == null) return null;

            int i = 0;
            while (i < Options.Length)
            {
                SpawnEntry entry = Options[i];
                if (entry != null && entry.EnemyPrefab != null)
                {
                    return entry;
                }
                i += 1;
            }

            return null;
        }
    }
}
