using UnityEngine;

namespace Dungeon
{
    public enum ArmorSlot
    {
        Body,
        Head,
        Legs,
        Shield
    }

    [CreateAssetMenu(fileName = "ArmorItem", menuName = "Dungeon/Item/Armor")]
    public class ArmorItem : ItemBase
    {
        [Header("Armor Stats")]
        public int DefenseBonus = 3;
        public ArmorSlot Slot = ArmorSlot.Body;

        private void OnEnable()
        {
            Category = ItemCategory.Armor;
        }

        public override void Use(Player player)
        {
            if (player == null)
            {
                Debug.LogWarning("ArmorItem used with null player.");
                return;
            }

            player.EquipArmor(this);
            Debug.Log("Equipped armor: " + DisplayName + " in slot: " + Slot);
        }
    }
}
