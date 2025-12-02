using System;
using UnityEngine;

namespace Dungeon
{
    [CreateAssetMenu(fileName = "EnemyDatabase", menuName = "Dungeon/Enemy Database")]
    public class EnemyDatabase : ScriptableObject
    {
        public EnemyEntry[] Enemies;
    }

    [Serializable]
    public class EnemyEntry
    {
        public string Id;
        public string DisplayName;
        public Combatant Prefab;
    }
}
