using System;
using UnityEngine;

namespace Dungeon
{
    public class TreasureRoom : Room
    {
        private bool _looted = false;

        private static int[] _possibleDice = new[] { 4, 6, 8, 10, 12, 14, 16, 24 };

        public override string RoomDescription()
        {
            return "A quiet chamber with dusty shelves. You see a faint glimmer in the corner.";
        }

        public override void OnRoomEntered(GameManager gm, Player player)
        {
            if (!Visited)
            {
                Debug.Log(RoomDescription());
                Debug.Log("It looks like there is something hidden here...");
                Visited = true;
            }
            else
            {
                Debug.Log("You return to the treasure chamber.");
            }
        }

        public override void OnRoomSearched(GameManager gm, Player player)
        {
            if (_looted)
            {
                Debug.Log("You have already searched this room. There is nothing left here.");
                return;
            }

            int count = gm.Rng.Next(1, 5);

            int[] gained = new int[count];
            for (int i = 0; i < count; i++)
            {
                int idx = gm.Rng.Next(0, _possibleDice.Length);
                int die = _possibleDice[idx];
                player.InventorySides.Add(die);
                gained[i] = die;
            }

            Debug.Log("You searched the room and found:");
            gm.PrintDiceBundle(gained);
            Debug.Log("Among the treasure, you also find an item...");
            gm.GrantRandomLoot(player, 1, 1, LootBias.WeaponsArmour);

            _looted = true;
        }

        public override void OnRoomExit(GameManager gm, Player player)
        {
            Debug.Log("You leave the room...");
        }
    }
}
