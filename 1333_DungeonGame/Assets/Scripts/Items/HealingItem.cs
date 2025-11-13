using UnityEngine;

namespace Dungeon
{
    [CreateAssetMenu(fileName = "HealingItem", menuName = "Dungeon/Item/Healing")]
    public class HealingItem : ItemBase
    {
        public int HealAmount = 20;
        private void OnEnable()
        {
            Category = ItemCategory.Healing;
        }

        public override void Use(Player player)
        {
            player.Heal(HealAmount);
            Debug.Log("Healed player by: " + HealAmount);
        }
    }
}
