using System;

namespace Dungeon
{
    public class CaveSlime : Enemy
    {
        public CaveSlime(Random rng)
            : base(
                name: "Cave Slime",
                level: 1,
                maxHp: 100,
                armour: 0,
                damageMin: 8,
                damageMax: 18,
                critChance: 10,
                critMultiplier: 160,
                diceSides: new int[] { 4, 4, 6, 8, 12 },
                rng: rng
            )
        { }
    }
}
