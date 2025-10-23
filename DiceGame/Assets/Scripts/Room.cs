using System;
using System.Numerics;

namespace Dungeon
{
    public abstract class Room
    {
        public Room? North;
        public Room? South;
        public Room? East;
        public Room? West;

        public bool Visited;
        public int VisitCount;

        /// <summary> Short Description of this room </summary>
        public abstract string RoomDescription();

        ///<summary> Called wehn player enter this room </summary>
        public abstract void OnRoomEntered(GameManager gm, Player player);

        ///<summary> Called when player searches the room. </summary>
        public abstract void OnRoomSearched(GameManager gm, Player player);

        /// <summary>Called when player exits this room.</summary>
        public abstract void OnRoomExit(GameManager gm, Player player);
    }
}
