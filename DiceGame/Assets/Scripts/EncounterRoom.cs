using System;

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
                Console.WriteLine(RoomDescription());
                Visited = true;
            }
            else
            {
                Console.WriteLine("You return to the Arena.");
            }

            if (Cleared)
            {
                Console.WriteLine("The arena is quiet. No foes remain.");
                Console.WriteLine();
                Console.WriteLine("Type: north/south/east/west to move, 'search' to search, 'inv' to check your inventory, 'exit' to end the game");
                return;
            }

            Console.WriteLine("An opponent appears! Time for a duel! (Poker Rules)");
            gm.StartEncounter();

            // If StartEncounter() killed the enemy, GameManager will mark this room cleared.
            // Next time you come back (including after closing inventory), no new spawn.

            Console.WriteLine();
            Console.WriteLine("Type: north/south/east/west to move, 'search' to search, 'inv' to check your inventory, 'exit' to end the game");
        }

        public override void OnRoomSearched(GameManager gm, Player player)
        {
            Console.WriteLine("You scout the arena, but there is nothing to loot here.");
        }

        public override void OnRoomExit(GameManager gm, Player player)
        {
            Console.WriteLine("You leave the room...");
        }
    }
}
