using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon
{
    public class EmptyRoom : Room
    {
        public override string RoomDescription()
        {
            return "You are in quiet, empty space. You sense no danger here.";
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
                Console.WriteLine("You return to the familiar empty room.");
            }
        }

        public override void OnRoomSearched(GameManager gm, Player player)
        {
            Console.WriteLine("You search the room but find only worthless dust.");
        }

        public override void OnRoomExit(GameManager gm, Player player)
        {
            Console.WriteLine("You quietly leave the empty room behind.");
        }
    }
}
