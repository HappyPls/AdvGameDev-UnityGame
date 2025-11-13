using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Dungeon
{
    public class Inventory : MonoBehaviour
    {
        [Header("Settings")]
        [Min(1)] public int Capacity = 20;

        [Header("Contents")]
        public List<InventorySlot> Slots = new List<InventorySlot>();

        void Awake()
        {
            if (Slots == null)
            {
                Slots = new List<InventorySlot>();
            }

            if (Slots.Count < Capacity)
            {
                int toAdd = Capacity - Slots.Count;
                int i = 0;
                while (i < toAdd)
                {
                    Slots.Add(new InventorySlot());
                    i += 1;
                }
            }
        }

        public bool AddItem(ItemBase item, int amount, ItemRarity rarity)
        {
            if (item == null || amount < 0) return false;

            int remaining = amount;

            if (item.IsStackable)
            {
                int i = 0;
                while (i < Slots.Count)
                {
                    InventorySlot slot = Slots[i];
                    if (!slot.IsEmpty && slot.Item == item && slot.Rarity == rarity && slot.Quantity < item.MaxStack)
                    {
                        int space = item.MaxStack - slot.Quantity;
                        int toAdd = Mathf.Min(space, remaining);
                        slot.Quantity += toAdd;
                        remaining -= toAdd;

                        if (remaining <= 0)
                        {
                            return true;
                        }
                    }
                    i += 1;
                }
            }
            int j = 0;
            while (j < Slots.Count && remaining > 0)
            {
                InventorySlot slot = Slots[j];
                if (slot.IsEmpty)
                {
                    slot.Item = item;
                    slot.Rarity = rarity;

                    if (item.IsStackable)
                    {
                        int toAdd = Mathf.Min(item.MaxStack, remaining);
                        slot.Quantity = toAdd;
                        remaining -= toAdd;
                    }
                    else
                    {
                        slot.Quantity = 1;
                        remaining -= 1;
                    }
                }
                j += 1;
            }

            return remaining <= 0;
        }
        public bool AddItem(ItemBase item, int amount)
        {
            return AddItem(item, amount, ItemRarity.Common);
        }

        public bool RemoveItem(ItemBase item, int amount)
        {
            if (item == null || amount <= 0) return false;

            int remaining = amount;
            int i = 0;
            while (i < Slots.Count)
            {
                InventorySlot slot = Slots[i];
                if (!slot.IsEmpty && slot.Item == item && slot.Quantity > 0)
                {
                    int toRemove = Mathf.Min(slot.Quantity, remaining);
                    slot.Quantity -= toRemove;
                    remaining -= toRemove;

                    if (slot.Quantity <= 0)
                    {
                        slot.Clear();
                    }

                    if (remaining <= 0)
                    {
                        return true;
                    }
                }
                i += 1;
            }

            return false;
        }

        public bool HasItem(ItemBase item, int amount)
        {
            if (item == null || amount <= 0) return false;

            int count = 0;
            int i = 0;
            while (i < Slots.Count)
            {
                InventorySlot slot = Slots[i];
                if (!slot.IsEmpty && slot.Item == item)
                {
                    count += slot.Quantity;
                    if (count >= amount) return true;
                }
                i += 1;
            }
            return false;
        }

        public InventorySlot GetFirstSlotWithItem(ItemBase item)
        {
            if (item == null) return null;
            for (int i = 0; i < Slots.Count; i++)
            {
                if (Slots[i].Item == item && Slots[i].Quantity > 0)
                    return Slots[i];
            }
            return null;
        }
    }
}
