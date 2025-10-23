using System;

namespace Dungeon
{
    public class SafeRoom : Room
    {
        private bool _used = false;

        public override string RoomDescription()
        {
            return "A calm, quiet chamber lit by faint glowing runes. You feel safe here.";
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
                Console.WriteLine("You return to the resting chamber. It’s still peaceful here.");
            }

            if (!_used)
            {
                int healAmount = gm.Rng.Next(15, 35);
                Console.WriteLine($"You take a moment to rest... You recover {healAmount} HP.");
                player.Heal(healAmount);
                _used = true;
            }
            else
            {
                Console.WriteLine("You’ve already rested here. The magic feels weaker now.");
            }

            Console.WriteLine();
            Console.WriteLine("Type: north/south/east/west to move, 'search' to search, 'inv' for inventory, 'exit' to quit.");
        }

        public override void OnRoomSearched(GameManager gm, Player player)
        {
            if (!_used)
            {
                Item heal = LootSystem.RandomHealing(gm.Rng);
                gm.GiveItem(player, heal);
                Console.WriteLine("You find a restorative: " + heal.Name + ".");
            }
            else
            {
                Console.WriteLine("There’s nothing else to find here.");
            }
        }

        public override void OnRoomExit(GameManager gm, Player player)
        {
            Console.WriteLine("You leave the peaceful chamber, feeling renewed.");
        }
    }
}
