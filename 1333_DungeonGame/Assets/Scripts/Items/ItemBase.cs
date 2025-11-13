using UnityEngine;

namespace Dungeon
{ 
    public enum ItemRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }
    public enum ItemCategory
    {
        Misc,
        Healing,
        Weapon,
        Armor
    }

    [CreateAssetMenu(fileName = "NewItem", menuName = "Dungeon/Item")]
    public class ItemBase : ScriptableObject
    {
        [Header("Identity")]
        public string ID;
        public string DisplayName = "New Item";

        [Header("Category")]
        public ItemCategory Category = ItemCategory.Misc;

        [TextArea(2, 4)]
        public string Description;

        [Header("Visual")]
        public Sprite Icon;

        [Header("Stacking")]
        [Min(1)] public int MaxStack = 99;
        public bool IsStackable = true;

        [Header("Gameplay")]
        public ItemRarity Rarity = ItemRarity.Common;
        public int GoldValue = 0;

        public virtual void Use(Player player)
        {
            Debug.Log("Used item: " + DisplayName + " on player: " + (player != null ? player.DisplayName : "null"));
        }
    }
}