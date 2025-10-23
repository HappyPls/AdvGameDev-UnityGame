using System;

namespace Dungeon
{
    // Simple dice roller + poker hand evaluator for 5 dice.
    public static class DiceRoller
    {
        private static Random _rng = new Random();

        // Roll one die
        public static int RollDie(int sides)
        {
            if (sides < 2) sides = 2;
            return _rng.Next(1, sides + 1);
        }

        // Roll a set of dice where each element is the number of sides for that die.
        public static int[] RollDice(int[] diceSides)
        {
            if (diceSides == null || diceSides.Length == 0)
                diceSides = new int[] { 6, 6, 6, 6, 6 };

            int count = diceSides.Length;
            int[] rolls = new int[count];

            for (int i = 0; i < count; i++)
            {
                int sides = diceSides[i];
                rolls[i] = RollDie(sides);
            }

            return rolls;
        }

        // A simple result object for poker scoring
        public class PokerResult
        {
            // e.g. "Two Pair"
            public string Name = string.Empty;
            // higher is better
            public int Rank;
            // percent, e.g. 150 means 1.5x damage
            public int Multiplier;
        }

        // Evaluate a 5-die poker hand and return a multiplier.
        // Categories (best to worst):
        // Five of a Kind, Four of a Kind, Full House, Straight, Three of a Kind, Two Pair, One Pair, High Card
        public static PokerResult Evaluate5(int[] rolls)
        {
            int n = rolls.Length;
            int[] r = new int[n];
            for (int i = 0; i < n; i++) r[i] = rolls[i];

            Array.Sort(r); // sort it by ascending order

            // Build counts of identical values (since dice can be different sizes, equal face values still count)

            int[] unique = new int[5];
            int[] counts = new int[5];
            int u = 0;

            for (int i = 0; i < n; i++)
            {
                if (u == 0)
                {
                    unique[u] = r[i];
                    counts[u] = 1;
                    u++;
                }
                else
                {
                    if (r[i] == unique[u - 1])
                    {
                        counts[u - 1] += 1;
                    }
                    else
                    {
                        unique[u] = r[i];
                        counts[u] = 1;
                        u++;
                    }
                }
            }

            // Find max count and also check for pair/triple combos
            int maxCount = 0;
            int pairs = 0;
            bool hasThree = false;

            for (int i = 0; i < u; i++)
            {
                if (counts[i] > maxCount) maxCount = counts[i];
                if (counts[i] == 2) pairs += 1;
                if (counts[i] == 3) hasThree = true;
            }

            bool isStraight = IsStraight(r);

            // Decide category + multiplier (tweak numbers as you like)
            PokerResult result = new PokerResult();

            if (maxCount == 5)
            {
                result.Name = "Five of a Kind";
                result.Rank = 8;
                result.Multiplier = 500;   // 5.0x
            }
            else if (maxCount == 4)
            {
                result.Name = "Four of a Kind";
                result.Rank = 7;
                result.Multiplier = 300;   // 3.0x
            }
            else if (hasThree && pairs == 1)
            {
                result.Name = "Full House";
                result.Rank = 6;
                result.Multiplier = 200;   // 2.0x
            }
            else if (isStraight)
            {
                result.Name = "Straight";
                result.Rank = 5;
                result.Multiplier = 175;   // 1.75x
            }
            else if (hasThree)
            {
                result.Name = "Three of a Kind";
                result.Rank = 4;
                result.Multiplier = 150;   // 1.5x
            }
            else if (pairs == 2)
            {
                result.Name = "Two Pair";
                result.Rank = 3;
                result.Multiplier = 125;   // 1.25x
            }
            else if (pairs == 1)
            {
                result.Name = "One Pair";
                result.Rank = 2;
                result.Multiplier = 110;   // 1.10x
            }
            else
            {
                result.Name = "High Card";
                result.Rank = 1;
                result.Multiplier = 100;   // 1.0x
            }

            return result;
        }

        // Straight = five consecutive values (like 2,3,4,5,6).
        private static bool IsStraight(int[] sorted)
        {
            // Remove duplicates into a small buffer
            int[] vals = new int[5];
            int len = 0;

            for (int i = 0; i < sorted.Length; i++)
            {
                if (len == 0 || sorted[i] != vals[len - 1])
                {
                    vals[len] = sorted[i];
                    len++;
                }
            }

            if (len != 5) return false; // must be 5 distinct numbers

            // Check consecutive
            for (int i = 1; i < len; i++)
            {
                if (vals[i] != vals[i - 1] + 1)
                    return false;
            }
            return true;
        }

        // Print the roll nicely, e.g. "Rolls: 2 3 3 5 6"
        public static void PrintRolls(int[] rolls)
        {
            Console.Write("Rolls: ");
            for (int i = 0; i < rolls.Length; i++)
            {
                Console.Write(rolls[i]);
                if (i < rolls.Length - 1) Console.Write(" ");
            }
            Console.WriteLine();
        }
    }
}
