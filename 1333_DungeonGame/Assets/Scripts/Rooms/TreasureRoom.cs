using UnityEngine;
using static UnityEditor.Progress;

namespace Dungeon
{
    public class TreasureRoom : RoomBase
    {
        [Header("Treasure Settings")]
        public LootTable LootTable;
        public int GoldReward = 0;
        public int ItemCount = 1;

        public override void TriggerRoomEvent(Player player)
        {
            if (IsCleared)
            {
                Debug.Log("Treasure already claimed.");
                return;
            }

            if (player == null)
            {
                Debug.LogWarning("TreasureRoom: No player passed in.");
                IsCleared = true;
                return;
            }


            if (LootTable == null)
            {
                Debug.LogWarning("TreasureRoom: No LootTable assigned.");
                IsCleared = true;
                return;
            }

            if (GoldReward > 0)
            {
                player.AddGold(GoldReward);
            }

            int i = 0;
            while (i < ItemCount)
            {
                ItemDrop drop;
                bool ok = LootTable.TryRoll(out drop);
                if (ok && drop.Item != null)
                {
                    player.AddItem(drop.Item, 1);

                    float mul = LootTable.GetStatMultiplier(drop.Rarity);
                    Debug.Log(
                        "TreasureRoom: Gave player a " + drop.Rarity + " " + drop.Item.DisplayName + " (multiplier " + mul + ")");
                }
                else
                {
                    Debug.LogWarning("TreasureRoom: LootTable returned no item.");
                }

                i += 1;
            }

            IsCleared = true;
        }
    }
}