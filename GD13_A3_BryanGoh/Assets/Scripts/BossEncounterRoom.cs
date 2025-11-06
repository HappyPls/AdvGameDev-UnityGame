using System;

namespace Dungeon
{
    public class BossEncounterRoom : Room
    {
        public bool Cleared;

        public override string RoomDescription()
        {
            return "An ominous chamber echoes with heavy steps.";
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
                Console.WriteLine("You return to the ominous chamber.");
            }

            if (Cleared)
            {
                Console.WriteLine("The chamber is quiet. No foes remain.");
                Console.WriteLine();
                Console.WriteLine("Type: north/south/east/west to move, 'search' to search, 'inv' to check your inventory, 'exit' to end the game");
                return;
            }

            EnemySpawner.ForceGolemSpawn();
            Console.WriteLine("A colossal presence looms...");
            gm.StartEncounter();

            var room = gm != null ? gm.GetType() : null; // no-op to keep structure; cleared is set by GameManager after kill

            Console.WriteLine();
            Console.WriteLine("Type: north/south/east/west to move, 'search' to search, 'inv' to check your inventory, 'exit' to end the game");
        }

        public override void OnRoomSearched(GameManager gm, Player player)
        {
            Console.WriteLine("Nothing to loot here.");
        }

        public override void OnRoomExit(GameManager gm, Player player)
        {
            Console.WriteLine("You leave the chamber...");
        }
    }
}
