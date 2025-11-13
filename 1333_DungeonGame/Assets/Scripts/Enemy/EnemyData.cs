using UnityEngine;

namespace Dungeon
{
    [CreateAssetMenu(menuName = "Dungeon/Enemy Data", fileName = "NewEnemyData")]
    public class EnemyData : ScriptableObject
    {
        [Header("Identity")]
        public string DisplayName = "Enemy";
        public int Level = 1;

        [Header("Core Stats")]
        public int MaxHp = 100;
        public int AttackPower = 10;
        public int Defense = 5;
        public int Speed = 10;

        [Header("Combat")]
        [Range(0f, 1f)] public float CritChance = 0.1f;
        [Range(0f, 2f)] public float CritMultiplier = 1.5f;
        [Range(0f, 1f)] public float DodgeChance = 0.05f;

        [Header("Dice System")]
        public Dice[] DiceSet;
        public int HandSize = 5;

        [Header("Rewards")]
        public int XPReward = 5;
        public int GoldRewardMin = 1;
        public int GoldRewardMax = 5;

        [Header("Boss")]
        public bool IsBoss = false;
    }
}
