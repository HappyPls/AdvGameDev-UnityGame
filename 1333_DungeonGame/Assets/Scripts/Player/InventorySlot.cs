using UnityEngine;
using System;

namespace Dungeon
{
    [Serializable]
    public class InventorySlot
    {
        public ItemBase Item;
        public ItemRarity Rarity;
        public int Quantity;

        public bool IsEmpty
        {
            get
            {
                return Item == null || Quantity <= 0;
            }
        }

        public void Clear()
        {
            Item = null;
            Rarity = ItemRarity.Common;
            Quantity = 0;
        }
    }
}
