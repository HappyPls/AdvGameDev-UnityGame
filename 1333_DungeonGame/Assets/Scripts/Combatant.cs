using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Dungeon
{
    /// <summary>
    /// Base combat unit for Player and Enemy.
    /// Shared stats, damage flow, and future dice/poker hooks.
    /// </summary>
    public abstract class  Combatant : MonoBehaviour
    {
        [Header("Identity")]
        public string DisplayName = "Combatant";
        public int Level = 1;

        [Header("Core Stats")]
        public int MaxHp = 100;
        public int HP = 100;
        public int AttackPower = 10;
        public int Defense = 5;
        public int Speed = 15;

        [Header("Combat")]
        [Range(0f, 1f)] public float CritChance = 0.1f;
        [Range(0f, 2f)] public float CritMultiplier = 1.5f;
        [Range(0f, 1f)] public float DodgeChance = 0.05f;

        [Header("Dice System")]
        public Dice[] DiceSet;
        public int HandSize = 5;

        [Header("Runtime State")]
        public bool IsAlive = true;
        public bool IsBusy = false;

        [Header("Events")]
        public UnityEvent OnDeath;
        public UnityEvent OnDamaged;
        public UnityEvent OnHealed;

        protected System.Random _rng;

        protected virtual void Awake()
        {
            _rng = new System.Random();
            if (HP > MaxHp) HP = MaxHp;
            if (HP < 0) IsAlive = false;
        }


        public virtual void Init(string nameOverride)
        {
            if (!string.IsNullOrEmpty(nameOverride))
            {
                DisplayName = nameOverride;
            }
            ClampHP();
        }

        public virtual int GetTotalAttackPower()
        {
            return AttackPower;
        }

        public virtual int GetTotalDefense()
        {
            return Defense;
        }

        public virtual void TakeTurn(Combatant target)
        {
            if (!IsAlive) return;
            if (target == null) return;

            AttackTarget(target);
        }

        public virtual void AttackTarget(Combatant target)
        {
            if (!IsAlive) return;
            if (target == null) return;

            int baseDamage = GetTotalAttackPower();

            bool didCrit = RollChance(CritChance);
            if (didCrit)
            {
                baseDamage = Mathf.RoundToInt(baseDamage * CritMultiplier);
            }

            target.ReceiveAttack(this, baseDamage, didCrit);
        }

        public virtual void ReceiveAttack(Combatant attacker, int rawDamage, bool wasCrit)
        {
            if(!IsAlive) return;

            if (RollChance(DodgeChance))
            {
                Debug.Log(DisplayName + " dodged the attack from " + attacker.DisplayName + "!");
                return;
            }

            int mitigated = Mathf.Max(1, rawDamage - GetTotalDefense());
            HP -= mitigated;
            if (OnDamaged != null) OnDamaged.Invoke();

            Debug.Log(attacker.DisplayName + " hit " + DisplayName + " for " + mitigated + (wasCrit ? " (CRIT)" : "") + ". HP: " + HP + "/" + MaxHp);

            if (HP <= 0)
            {
                Die();
            }
        }
        public virtual void Heal(int amount)
        {
            if (!IsAlive) return;
            if (amount <= 0) return;

            int before = HP;
            HP += amount;
            ClampHP();
            int actual = HP - before;

            if (actual > 0)
            {
                if (OnHealed != null) OnHealed.Invoke();
                Debug.Log(DisplayName + " healed " + actual + " HP. HP: " + HP + "/" + MaxHp);
            }
        }
        public virtual void Die()
        {
            if (!IsAlive) return;
            IsAlive = false;
            HP = 0;

            Debug.Log(DisplayName + " has fallen.");
            if (OnDeath != null) OnDeath.Invoke();
        }
        protected void ClampHP()
        {
            if (HP > MaxHp) HP = MaxHp;
            if (HP < 0) HP = 0;
        }

        protected bool RollChance(float chance)
        {
            if (chance <= 0f) return false;
            if (chance >= 1f) return true;

            double r = _rng.NextDouble();
            return r < chance;
        }
        public virtual int[] RollDiceHand()
        {
            if (DiceSet == null || DiceSet.Length == 0)
            {
                Debug.LogWarning(DisplayName + " has no dice assigned!");
                return new int[0];
            }

            int count = Mathf.Max(1, HandSize);
            int[] hand = new int[count];
            for (int i = 0; i < count; i++)
            {
                Dice die = DiceSet[i % DiceSet.Length];
                hand[i] = die.Roll(_rng);
            }
            return hand;
        }
        public virtual void TakeDamage(int amount)
        {
            if (!IsAlive) return;
            int dmg = Mathf.Max(1, amount);
            HP -= dmg;
            if (OnDamaged != null) OnDamaged.Invoke();
            Debug.Log(DisplayName + " took " + dmg + " damage. HP: " + HP + "/" + MaxHp);
            if (HP <= 0) Die();
        }
        public virtual void TakeMitigatedDamage(int amount)
        {
            if (!IsAlive) return;
            int mitigated = Mathf.Max(1, amount - Defense);
            HP -= mitigated;
            if (OnDamaged != null) OnDamaged.Invoke();
            Debug.Log(DisplayName + " took " + mitigated + " mitigated dmg. HP: " + HP + "/" + MaxHp);
            if (HP <= 0) Die();
        }
    }
}
