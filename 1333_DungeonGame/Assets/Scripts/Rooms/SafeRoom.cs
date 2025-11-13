using UnityEngine;

namespace Dungeon
{
    public class SafeRoom : RoomBase
    {
        public int HealAmount = 10;

        public override void TriggerRoomEvent(Player player)
        {
            if (IsCleared) return;
            if (player != null)
            {
                player.Heal(HealAmount);
                Debug.Log("SafeRoom: Rested and healed +" + HealAmount);
            }
            IsCleared = true;
        }
    }
}
