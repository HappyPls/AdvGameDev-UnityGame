using UnityEngine;

namespace Dungeon
{
    public class Player : Combatant
    {
        [Header("Player-Specific")]
        public int Experience = 0;
        public int Gold = 0;

        [Header("Inventory")]
        public Inventory Inventory;

        [Header("Equipment - Weapon")]
        public WeaponItem EquippedWeapon;
        public ItemRarity EquippedWeaponRarity = ItemRarity.Common;

        [Header("Equipment - Armor")]
        public ArmorItem EquippedBodyArmor;
        public ItemRarity EquippedBodyRarity = ItemRarity.Common;

        public ArmorItem EquippedHeadArmor;
        public ItemRarity EquippedHeadRarity = ItemRarity.Common;

        public ArmorItem EquippedLegArmor;
        public ItemRarity EquippedLegRarity = ItemRarity.Common;

        public ArmorItem EquippedShieldArmor;
        public ItemRarity EquippedShieldRarity = ItemRarity.Common;

        [Header("Loot Config (for rarity multipliers)")]
        public LootTable LootConfig;

        protected override void Awake()
        {
            base.Awake();

            if (string.IsNullOrEmpty(DisplayName))
            {
                DisplayName = "Player";
            }

            if (Inventory == null)
            {
                Inventory = GetComponent<Inventory>();
            }
        }

        public override int GetTotalAttackPower()
        {
            int total = AttackPower;

            if (EquippedWeapon != null)
            {
                float mul = 1f;
                if (LootConfig != null)
                {
                    mul = LootConfig.GetStatMultiplier(EquippedWeaponRarity);
                }

                float bonus = EquippedWeapon.AttackBonus * mul;
                total += Mathf.RoundToInt(bonus);
            }

            return total;
        }

        public override int GetTotalDefense()
        {
            int total = Defense;

            if (LootConfig != null)
            {
                // Body armor
                if (EquippedBodyArmor != null)
                {
                    float mul = LootConfig.GetStatMultiplier(EquippedBodyRarity);
                    float bonus = EquippedBodyArmor.DefenseBonus * mul;
                    total += Mathf.RoundToInt(bonus);
                }

                // Head armor
                if (EquippedHeadArmor != null)
                {
                    float mul = LootConfig.GetStatMultiplier(EquippedHeadRarity);
                    float bonus = EquippedHeadArmor.DefenseBonus * mul;
                    total += Mathf.RoundToInt(bonus);
                }

                // Leg armor
                if (EquippedLegArmor != null)
                {
                    float mul = LootConfig.GetStatMultiplier(EquippedLegRarity);
                    float bonus = EquippedLegArmor.DefenseBonus * mul;
                    total += Mathf.RoundToInt(bonus);
                }

                // Shield
                if (EquippedShieldArmor != null)
                {
                    float mul = LootConfig.GetStatMultiplier(EquippedShieldRarity);
                    float bonus = EquippedShieldArmor.DefenseBonus * mul;
                    total += Mathf.RoundToInt(bonus);
                }
            }
            else
            {
                if (EquippedBodyArmor != null)
                {
                    total += EquippedBodyArmor.DefenseBonus;
                }
                if (EquippedHeadArmor != null)
                {
                    total += EquippedHeadArmor.DefenseBonus;
                }
                if (EquippedLegArmor != null)
                {
                    total += EquippedLegArmor.DefenseBonus;
                }
                if (EquippedShieldArmor != null)
                {
                    total += EquippedShieldArmor.DefenseBonus;
                }
            }

            return total;
        }

        public void AddGold(int amount)
        {
            if (amount <= 0) return;
            Gold += amount;
            Debug.Log(DisplayName + " gained " + amount + " gold. Total: " + Gold);
        }

        public void AddItem(ItemBase item, int amount, ItemRarity rarity)
        {
            if (Inventory == null)
            {
                Debug.LogWarning("Player has no Inventory component.");
                return;
            }

            bool ok = Inventory.AddItem(item, amount, rarity);
            if (ok)
            {
                Debug.Log(
                    "Player received " + amount + " x " + rarity + " " + item.DisplayName
                );
            }
            else
            {
                Debug.LogWarning("Inventory full, could not add item: " + item.DisplayName);
            }
        }

        public void AddItem(ItemBase item, int amount)
        {
            AddItem(item, amount, ItemRarity.Common);
        }

        public void EquipWeapon(WeaponItem weapon, ItemRarity rarity)
        {
            EquippedWeapon = weapon;
            EquippedWeaponRarity = rarity;

            string weaponName = weapon != null ? weapon.DisplayName : "None";
            Debug.Log("Player equipped weapon: " + weaponName + " [" + rarity + "]");
        }

        public void EquipWeapon(WeaponItem weapon)
        {
            EquipWeapon(weapon, ItemRarity.Common);
        }

        public void EquipArmor(ArmorItem armor, ItemRarity rarity)
        {
            if (armor == null)
            {
                Debug.LogWarning("EquipArmor called with null armor.");
                return;
            }

            if (armor.Slot == ArmorSlot.Body)
            {
                EquippedBodyArmor = armor;
                EquippedBodyRarity = rarity;
            }
            else if (armor.Slot == ArmorSlot.Head)
            {
                EquippedHeadArmor = armor;
                EquippedHeadRarity = rarity;
            }
            else if (armor.Slot == ArmorSlot.Legs)
            {
                EquippedLegArmor = armor;
                EquippedLegRarity = rarity;
            }
            else if (armor.Slot == ArmorSlot.Shield)
            {
                EquippedShieldArmor = armor;
                EquippedShieldRarity = rarity;
            }

            Debug.Log(
                "Player equipped armor: " +
                armor.DisplayName + " in slot: " + armor.Slot +
                " [" + rarity + "]"
            );
        }

        public void EquipArmor(ArmorItem armor)
        {
            EquipArmor(armor, ItemRarity.Common);
        }

        public override void Die()
        {
            base.Die();

            if (GameManager.Exists())
            {
                GameManager.Instance.OnPlayerDied(this);
            }
        }
    }
}
