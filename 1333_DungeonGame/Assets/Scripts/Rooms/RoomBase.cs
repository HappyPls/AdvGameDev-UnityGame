using UnityEngine;

namespace Dungeon
{
    public abstract class RoomBase : MonoBehaviour
    {
        [Header("Room Information")]
        public string RoomName;
        public Vector2Int GridPosition;
        public bool IsCleared = false;

        [Header("Room State")]
        public bool IsActive = false;

        public virtual void Init(Vector2Int gridPos)
        {
            GridPosition = gridPos;
            RoomName = GetType().Name;
        }

        public virtual void EnterRoom(Player player)
        {
            Debug.Log($"Player entered {RoomName} at {GridPosition}");
            IsActive = true;
        }

        public virtual void ExitRoom(Player player)
        {
            Debug.Log($"Player exited {RoomName} at {GridPosition}");
            IsActive = false;
        }

        public abstract void TriggerRoomEvent(Player player);
    }
}