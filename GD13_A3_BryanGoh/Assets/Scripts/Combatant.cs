using System;

namespace Dungeon
{
    public class Combatant
    {
        // Basic information
        public string Name { get; protected set; }
        public int Level { get; protected set; }

        // Vital stats for health and defense
        public int MaxHp { get; protected set; }
        public int HP { get; protected set; }
        public int Armour { get; protected set; }

        // Offensive stats for damage and criticals
        public int DamageMin { get; protected set; }
        public int DamageMax { get; protected set; }
        public int CritChance { get; protected set; }
        public int CritMultiplier { get; protected set; }

        // Dice sides used for special rolls or actions
        public int[] DiceSides { get; protected set; }
        protected Random Rng;

        // Armour DR tuning (instance-level, not const)
        protected double ArmourCon;
        protected double MaxDR;

        //initializes the combatant’s stats and dice
        public Combatant(
            string name,
            int level,
            int maxHp,
            int hP,
            int armour,
            int damageMin,
            int damageMax,
            int critChance,
            int critMultiplier,
            int[] diceSides,
            Random rng)
        {
            Name = name;
            Level = level;
            MaxHp = maxHp;
            HP = MaxHp;
            Armour = armour;
            DamageMin = damageMin;
            DamageMax = damageMax;
            CritChance = critChance;
            CritMultiplier = critMultiplier;
            DiceSides = diceSides != null ? (int[])diceSides.Clone() : new int[] { 6, 8, 12, 20 };
            Rng = rng ?? new Random();

            ArmourCon = 50.0;
            MaxDR = 0.75;
        }

        //check if combatant is still alive
        public bool IsAlive => HP > 0;

        // Handles basic attack logic
        public virtual void Attack(Combatant target, int multiplierPercent)
        {
            int raw = RollDamage();
            if (multiplierPercent < 0) multiplierPercent = 0;
            raw = raw * multiplierPercent / 100;

            // check the actual damage after mitigation
            int shown = target.CheckMitigatedDamage(raw);

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(Name);
            Console.ResetColor();
            Console.Write(" attacks ");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(target.Name);
            Console.ResetColor();

            Console.Write(" for ");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(shown);
            Console.ResetColor();

            Console.WriteLine(" damage!");

            // Apply the damage (no penetration by default)
            target.TakeDamage(raw);
        }

        // Determines base and critical damage
        protected virtual int RollDamage()
        {
            int dmg = Rng.Next(DamageMin, DamageMax + 1);
            int roll = Rng.Next(1, 101);

            // Apply critical multiplier if chance succeeds
            if (roll <= CritChance)
            {
                dmg = dmg * CritMultiplier / 100;
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Critical hit!");
                Console.ResetColor();
            }

            return dmg;
        }

        // Applies incoming damage after armour reduction (optional penetration)
        public virtual void TakeDamage(int amount, int flatPen = 0, double pctPen = 0.0)
        {
            int reduced = MitigatedDamage(amount, Armour, flatPen, pctPen);
            HP -= reduced;
            if (HP < 0) HP = 0;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(Name);
            Console.ResetColor();
            Console.Write(" takes ");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(reduced);
            Console.ResetColor();
            Console.WriteLine(" damage (" + HP + "/" + MaxHp + ")");
        }

        public int CheckMitigatedDamage(int incoming, int flatPen = 0, double pctPen = 0.0)
        {
            return MitigatedDamage(incoming, Armour, flatPen, pctPen);
        }

        // Restores HP, capped at MaxHp
        public void Heal(int amount)
        {
            HP += amount;
            if (HP > MaxHp) HP = MaxHp;
        }

        // Base Mitigation, set at a min DR of 1.
        protected virtual int MitigatedDamage(int incoming, int armour, int flatPen, double pctPen)
        {
            if (incoming <= 0) return 0;

            // Effective armour after penetration
            double effArmour = System.Math.Max(0.0, armour - flatPen);
            pctPen = System.Math.Max(0.0, System.Math.Min(pctPen, 0.95)); // clamp 0..0.95 so pct pen can’t zero armour
            effArmour *= (1.0 - pctPen);

            double dr = effArmour <= 0.0 ? 0.0 : (effArmour / (effArmour + ArmourCon));
            dr = System.Math.Min(dr, MaxDR);

            int mitigated = (int)System.Math.Ceiling(incoming * (1.0 - dr));
            if (mitigated < 1) mitigated = 1; // never 0
            return mitigated;
        }
    }
}
