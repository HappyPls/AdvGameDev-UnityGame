using System;

namespace Dungeon
{
    public class IronGolem : Enemy
    {
        public IronGolem(Random rng)
            : base(
                name: "Iron Golem",
                level: 4,
                maxHp: 220,
                armour: 16,
                damageMin: 18,
                damageMax: 30,
                critChance: 3,
                critMultiplier: 140,
                diceSides: new int[] { 8, 8, 10, 10, 12 },
                rng: rng
            )
        { }
    }
}

