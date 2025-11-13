using UnityEngine;

namespace Dungeon
{
    [CreateAssetMenu(fileName = "WeaponItem", menuName = "Dungeon/Item/Weapon")]
    public class WeaponItem : ItemBase
    {
        [Header("Weapon Stats")]
        public int AttackBonus = 5;

        private void OnEnable()
        {
            Category = ItemCategory.Weapon;
        }

        public override void Use(Player player)
        {
            if (player == null)
            {
                Debug.LogWarning("WeaponItem used with null player.");
                return;
            }

            player.EquipWeapon(this);
            Debug.Log("Equipped weapon: " + DisplayName);
        }
    }
}
