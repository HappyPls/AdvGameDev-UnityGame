using UnityEngine;

namespace Dungeon
{
    [System.Serializable]
    public struct ItemDrop
    {
        public ItemBase Item;
        public ItemRarity Rarity;
    }

    [CreateAssetMenu(fileName = "Loot Table", menuName = "Dungeon/Loot Table")]
    public class LootTable : ScriptableObject
    {
        [Header("All possible items available")]
        public ItemBase[] Items;

        [Header("Rarity Weights")]
        [Min(0f)] public float CommonWeight = 60f;
        [Min(0f)] public float UncommonWeight = 25f;
        [Min(0f)] public float RareWeight = 10f;
        [Min(0f)] public float EpicWeight = 4f;
        [Min(0f)] public float LegendaryWeight = 1f;

        [Header("Stat Multipliers")]
        public float CommonMultiplier = 1.0f;
        public float UncommonMultiplier = 1.1f;
        public float RareMultiplier = 1.25f;
        public float EpicMultiplier = 1.5f;
        public float LegendaryMultiplier = 2.0f;

        System.Random _rng;

        void OnEnable()
        {
            if (_rng == null)
            {
                _rng = new System.Random();
            }
        }

        public bool TryRoll(out ItemDrop drop)
        {
            drop = new ItemDrop();

            if (Items == null || Items.Length == 0)
            {
                Debug.LogWarning("LootTable: No items assigned.");
                return false;
            }

            //Pick random base item
            int index = _rng.Next(0, Items.Length);
            ItemBase baseItem = Items[index];
            if (baseItem == null)
            {
                Debug.LogWarning("LootTable: Null item in list.");
                return false;
            }

            //Roll rarity by weight
            ItemRarity rarity = RollRarity();

            drop.Item = baseItem;
            drop.Rarity = rarity;
            return true;
        }

        public ItemRarity RollRarity()
        {
            float total = CommonWeight + UncommonWeight + RareWeight + EpicWeight + LegendaryWeight;
            if (total <= 0f)
            {
                return ItemRarity.Common;
            }

            double r = _rng.NextDouble() * total;

            if (r < CommonWeight) return ItemRarity.Common;
            r -= CommonWeight;

            if (r < UncommonWeight) return ItemRarity.Uncommon;
            r -= UncommonWeight;

            if (r < RareWeight) return ItemRarity.Rare;
            r -= RareWeight;

            if (r < EpicWeight) return ItemRarity.Epic;

            return ItemRarity.Legendary;
        }

        public float GetStatMultiplier(ItemRarity rarity)
        {
            if (rarity == ItemRarity.Common) return CommonMultiplier;
            if (rarity == ItemRarity.Uncommon) return UncommonMultiplier;
            if (rarity == ItemRarity.Rare) return RareMultiplier;
            if (rarity == ItemRarity.Epic) return EpicMultiplier;
            if (rarity == ItemRarity.Legendary) return LegendaryMultiplier;

            return 1f;
        }
    }
}
