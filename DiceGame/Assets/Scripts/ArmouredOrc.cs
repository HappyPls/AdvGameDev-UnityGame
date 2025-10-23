using System;

namespace Dungeon
{
    public class ArmouredOrc : Enemy
    {
        public ArmouredOrc(Random rng)
            : base(
                name: "Armoured Orc",
                level: 3,
                maxHp: 150,
                armour: 10,
                damageMin: 14,
                damageMax: 24,
                critChance: 5,
                critMultiplier: 150,
                diceSides: new int[] { 6, 8, 10, 10, 12 },
                rng: rng
            )
        { }
    }
}