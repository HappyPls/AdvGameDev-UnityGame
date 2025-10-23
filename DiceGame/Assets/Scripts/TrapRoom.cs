using System;

namespace Dungeon
{
    public class TrapRoom : Room
    {
        private bool _triggered = false;

        // Possible dice rewards
        private static readonly int[] _possibleDice = new[] { 4, 6, 8, 10, 12, 20 };

        public override string RoomDescription()
        {
            return "The floor creaks slightly under your feet... this room hides a trap.";
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
                Console.WriteLine("You cautiously re-enter the trapped chamber...");
            }

            //Initial chance of falling into the trap
            if (!_triggered)
            {
                int triggerChance = gm.Rng.Next(1, 101); // 1–100
                if (triggerChance <= 40) // 40% chance to trigger immediately
                {
                    int dmg = gm.Rng.Next(8, 15);
                    Console.WriteLine("You step on a loose stone — arrows shoot from the walls!");
                    player.TakeDamage(dmg);
                    _triggered = true;
                }
                else
                {
                    Console.WriteLine("You notice a suspicious pressure plate underfoot... You avoid stepping on it.");
                    Console.WriteLine("Perhaps you can try to disarm it by searching the room.");
                }
            }
            else
            {
                Console.WriteLine("The trap here has already been triggered or disarmed.");
            }

            Console.WriteLine();
            Console.WriteLine("Type: north/south/east/west to move, 'search' to attempt disarm, 'inv' for inventory, 'exit' to quit.");
        }

        public override void OnRoomSearched(GameManager gm, Player player)
        {
            if (_triggered)
            {
                Console.WriteLine("The trap mechanism is already disabled.");
                return;
            }

            Console.WriteLine("You crouch down and carefully inspect the trap...");

            //Disarm attempt using a hidden d6 roll
            int disarmRoll = gm.Rng.Next(1, 7); // d6 roll (1–6)
            if (disarmRoll >= 4)
            {
                Console.WriteLine("You successfully disarm the mechanism! The trap is now safe.");
                _triggered = true;

                //Reward the player with one random die
                int idx = gm.Rng.Next(_possibleDice.Length);
                int rewardDie = _possibleDice[idx];
                player.InventorySides.Add(rewardDie);
                Console.WriteLine($"You salvage a small trinket from the trap — you gain a d{rewardDie}!");
                Item reward = LootSystem.RandomHealing(gm.Rng);
                gm.GiveItem(player, reward);
                Console.WriteLine("You salvage a vial from the trap’s kit — you gain: " + reward.Name + ".");
            }
            else
            {
                int dmg = gm.Rng.Next(6, 12);
                Console.WriteLine("Your hand slips — the trap snaps!");
                player.TakeDamage(dmg);
                _triggered = true;
            }
        }

        public override void OnRoomExit(GameManager gm, Player player)
        {
            Console.WriteLine("You leave the trapped room, relieved to be safe again.");
        }
    }
}
