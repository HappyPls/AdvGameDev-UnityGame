using UnityEngine;

namespace Dungeon
{
    public class Enemy : Combatant
    {
        [Header("Config")]
        public EnemyData Data;

        [Header("Runtime Flags")]
        public bool IsBoss = false;

        [Header("Rewards (Runtime from Data)")]
        public int XPReward = 5;
        public int GoldRewardMin = 1;
        public int GoldRewardMax = 5;

        protected override void Awake()
        {
            base.Awake();
            ApplyData();
            if (string.IsNullOrEmpty(DisplayName))
            {
                DisplayName = "Enemy";
            }
        }

        public void ApplyData()
        {
            if (Data == null) return;
            DisplayName = Data.DisplayName;
            Level = Data.Level;

            MaxHp = Data.MaxHp;
            HP = MaxHp;

            AttackPower = Data.AttackPower;
            Defense = Data.Defense;
            Speed = Data.Speed;

            CritChance = Data.CritChance;
            CritMultiplier = Data.CritMultiplier;
            DodgeChance = Data.DodgeChance;

            DiceSet = Data.DiceSet;
            HandSize = Data.HandSize;

            XPReward = Data.XPReward;
            GoldRewardMin = Data.GoldRewardMin;
            GoldRewardMax = Data.GoldRewardMax;

            IsBoss = Data.IsBoss;
        }

        public override void Die()
        {
            base.Die();

            if (GameManager.Exists())
            {
                if (IsBoss || GameManager.Instance.IsFinalBoss(this))
                {
                    GameManager.Instance.OnBossDefeated(this);
                }
            }
        }

        public override void TakeTurn(Combatant target)
        {
            base.TakeTurn(target);
        }

        public int RollGoldReward()
        {
            int minVal = Mathf.Max(0, GoldRewardMin);
            int maxVal = Mathf.Max(minVal, GoldRewardMax);
            if (minVal == maxVal) return minVal;
            int amount = _rng.Next(minVal, maxVal + 1);
            return amount;
        }
    }
}