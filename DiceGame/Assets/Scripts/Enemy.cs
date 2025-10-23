using System;

namespace Dungeon
{
    public class Enemy : Combatant
    {
        public Enemy(
            string name,
            int level,
            int maxHp,
            int armour,
            int damageMin,
            int damageMax,
            int critChance,
            int critMultiplier,
            int[] diceSides,
            Random rng
        )
        : base(
            name: name,
            level: level,
            maxHp: maxHp,
            hP: maxHp,
            armour: armour,
            damageMin: damageMin,
            damageMax: damageMax,
            critChance: critChance,
            critMultiplier: critMultiplier,
            diceSides: diceSides ?? new int[] { 6, 8, 12, 20 },
            rng: rng ?? new Random()
        )
        { }
    }
}
