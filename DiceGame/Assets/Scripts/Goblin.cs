using System;

namespace Dungeon
{
    public class Goblin : Enemy
    {
        public Goblin(Random rng)
            : base(
                name: "Goblin",
                level: 2,
                maxHp: 90,
                armour: 2,
                damageMin: 12,
                damageMax: 22,
                critChance: 25,
                critMultiplier: 175,
                diceSides: new int[] { 4, 6, 6, 8, 10 },
                rng: rng
            )
        { }
    }
}
