using System;

namespace Dungeon
{
    public enum ItemType
    {
        Weapon,
        Armour,
        Consumable
    }

    public abstract class Item
    {
        public string Name { get; protected set; }
        public ItemType Type { get; protected set; }

        protected Item(string name, ItemType type)
        {
            Name = string.IsNullOrWhiteSpace(name) ? "Item" : name.Trim();
            Type = type;
        }
    }

    // Weapons add to damage range
    public class Weapon : Item
    {
        public int MinBonus { get; protected set; }
        public int MaxBonus { get; protected set; }

        public Weapon(string name, int minBonus, int maxBonus)
            : base(name, ItemType.Weapon)
        {
            if (maxBonus < minBonus) maxBonus = minBonus;
            MinBonus = minBonus;
            MaxBonus = maxBonus;
        }
    }

    // Armour adds to armour value
    public class ArmourItem : Item
    {
        public int ArmourBonus { get; protected set; }

        public ArmourItem(string name, int armourBonus)
            : base(name, ItemType.Armour)
        {
            ArmourBonus = armourBonus;
        }
    }

    // Consumables: e.g., healing potion
    public class Consumable : Item
    {
        public int HealAmount { get; protected set; }

        public Consumable(string name, int healAmount)
            : base(name, ItemType.Consumable)
        {
            HealAmount = healAmount;
        }
    }
}
