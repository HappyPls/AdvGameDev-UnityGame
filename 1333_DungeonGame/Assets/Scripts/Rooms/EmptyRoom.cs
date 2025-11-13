using UnityEngine;

namespace Dungeon
{
    public class EmptyRoom : RoomBase
    {
        public override void TriggerRoomEvent(Player player)
        {
            Debug.Log("This room is empty. Nothing happens.");
            IsCleared = true;
        }
    }
}

