using UnityEngine;

namespace Dungeon
{
    public class EncounterRoom : Room
    {
        public bool Cleared;

        public override string RoomDescription()
        {
            return "A tense arena-like space. You feel watched.";
        }

        public override void OnRoomEntered(GameManager gm, Player player)
        {
            if (!Visited)
            {
                Debug.Log(RoomDescription());
                Visited = true;
            }
            else
            {
                Debug.Log("You return to the Arena.");
            }

            if (Cleared)
            {
                Debug.Log("The arena is quiet. No foes remain.");
                return;
            }

            Debug.Log("An opponent appears! Time for a duel! (Poker Rules)");
        }

        public override void OnRoomSearched(GameManager gm, Player player)
        {
            Debug.Log("You scout the arena, but there is nothing to loot here.");
        }

        public override void OnRoomExit(GameManager gm, Player player)
        {
            Debug.Log("You leave the room...");
        }
    }
}
