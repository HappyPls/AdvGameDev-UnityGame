using UnityEngine;

namespace Dungeon
{
    public class TrapRoom : RoomBase
    {
        [Header("Trap Settings")]
        public int TrapDamage = 10;
        public override void TriggerRoomEvent(Player player)
        {
            if (IsCleared)
            {
                Debug.Log("Trap already triggered.");
                return;
            }
            if (player != null)
            {
                player.TakeMitigatedDamage(TrapDamage);
                Debug.Log("Trap sprung! -" + TrapDamage + " HP");
            }
            IsCleared = true;
        }
    }
}


