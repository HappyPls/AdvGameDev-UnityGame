using System;

namespace Dungeon
{
    public enum LootBias
    {
        HealingOnly,      // RestRoom
        WeaponsArmour,    // TreasureRoom (leans to Weapons + Armour)
        ConsumablesLean,  // EncounterRoom (leans to healing items)
        Any
    }

    public static class LootSystem
    {
        // Healing consumables only
        private static readonly Consumable[] _heals = new Consumable[]
        {
            new Consumable("Small Potion", 15),
            new Consumable("Lesser Potion", 25),
            new Consumable("Potion", 35),
            new Consumable("Greater Potion", 50),
            new Consumable("Elixir", 80),
        };

        // Weapons (name, minBonus, maxBonus)
        private static readonly Weapon[] _weapons = new Weapon[]
        {
            new Weapon("Rusty Dagger", 1, 2),
            new Weapon("Short Sword", 2, 4),
            new Weapon("Greatsword", 3, 5),
            new Weapon("Warhammer", 4, 7),
            new Weapon("Enchanted Blade", 5, 9),
        };

        // Armour (name, armourBonus)
        private static readonly ArmourItem[] _armours = new ArmourItem[]
        {
            new ArmourItem("Cloth Wraps", 1),
            new ArmourItem("Leather Vest", 3),
            new ArmourItem("Chain Shirt", 5),
            new ArmourItem("Half Plate", 7),
            new ArmourItem("Runed Plate", 10),
        };

        public static Consumable RandomHealing(Random rng)
        {
            int i = rng.Next(_heals.Length);
            Consumable t = _heals[i];
            return new Consumable(t.Name, t.HealAmount);
        }

        public static Weapon RandomWeapon(Random rng)
        {
            int i = rng.Next(_weapons.Length);
            Weapon t = _weapons[i];
            return new Weapon(t.Name, t.MinBonus, t.MaxBonus);
        }

        public static ArmourItem RandomArmour(Random rng)
        {
            int i = rng.Next(_armours.Length);
            ArmourItem t = _armours[i];
            return new ArmourItem(t.Name, t.ArmourBonus);
        }

        // Weighted based on room bias
        public static Item RandomItem(Random rng, LootBias bias)
        {
            if (bias == LootBias.HealingOnly)
                return RandomHealing(rng);

            int wWeapon, wArmour, wHeal;
            if (bias == LootBias.WeaponsArmour)
            {
                wWeapon = 4; wArmour = 4; wHeal = 1;
            }
            else if (bias == LootBias.ConsumablesLean)
            {
                wWeapon = 2; wArmour = 2; wHeal = 5;
            }
            else
            {
                wWeapon = 3; wArmour = 3; wHeal = 3;
            }

            int total = wWeapon + wArmour + wHeal;
            int roll = rng.Next(total);

            if (roll < wWeapon) return RandomWeapon(rng);
            roll -= wWeapon;

            if (roll < wArmour) return RandomArmour(rng);
            return RandomHealing(rng);
        }
    }
}
