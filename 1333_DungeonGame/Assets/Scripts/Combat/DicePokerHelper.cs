using System;
using UnityEngine;

namespace Dungeon
{
    [Serializable]
    public struct DiceHandResult
    {
        public int[] DiceValues;
        public string HandType;
        public float DamageMultiplier;
    }

    public static class DicePokerHelper
    {
        public static DiceHandResult RollAndEvaluate(int diceCount)
        {
            if (diceCount <= 0) diceCount = 5;

            int[] dice = new int[diceCount];
            int i = 0;
            while (i < diceCount)
            {
                dice[i] = UnityEngine.Random.Range(1, 7);
                i += 1;
            }

            return EvaluateHand(dice);
        }

        public static DiceHandResult EvaluateHand(int[] dice)
        {
            DiceHandResult result = new DiceHandResult
            {
                DiceValues = dice,
                HandType = "HighCard",
                DamageMultiplier = 1.0f
            };

            if (dice == null || dice.Length == 0)
            {
                return result;
            }

            int[] counts = new int[7];
            int i = 0;
            while (i < dice.Length)
            {
                int v = dice[i];
                if (v >= 1 && v <= 6)
                {
                    counts[v] += 1;
                }
                i += 1;
            }

            int maxCount = 0;
            int distinct = 0;
            bool hasThree = false;
            bool hasPair = false;

            i = 1;
            while (i <= 6)
            {
                int c = counts[i];
                if (c > 0)
                {
                    distinct += 1;
                    if (c > maxCount) maxCount = c;
                    if (c == 3) hasThree = true;
                    if (c == 2) hasPair = true;
                }
                i += 1;
            }

            int[] sorted = (int[])dice.Clone();
            Array.Sort(sorted);

            bool isStraight = IsStraight(sorted);

            if (maxCount == 5)
            {
                result.HandType = "FiveOfAKind";
                result.DamageMultiplier = 3.0f;
            }
            else if (maxCount == 4)
            {
                result.HandType = "FourOfAKind";
                result.DamageMultiplier = 2.5f;
            }
            else if (hasThree && hasPair)
            {
                result.HandType = "FullHouse";
                result.DamageMultiplier = 2.0f;
            }
            else if (isStraight)
            {
                result.HandType = "Straight";
                result.DamageMultiplier = 1.8f;
            }
            else if (hasThree)
            {
                result.HandType = "ThreeOfAKind";
                result.DamageMultiplier = 1.8f;
            }
            else if (CountPairs(counts) == 2)
            {
                result.HandType = "TwoPair";
                result.DamageMultiplier = 1.3f;
            }
            else if (CountPairs(counts) == 1)
            {
                result.HandType = "OnePair";
                result.DamageMultiplier = 1.2f;
            }
            else
            {
                result.HandType = "HighCard";
                result.DamageMultiplier = 1.0f;
            }

            return result;
        }

        private static bool IsStraight(int[] sorted)
        {
            if (sorted == null || sorted.Length < 2) return false;

            int i = 1;
            while (i < sorted.Length)
            {
                if (sorted[i] != sorted[i - 1] + 1)
                {
                    return false;
                }
                i += 1;
            }

            return true;
        }

        private static int CountPairs(int[] counts)
        {
            int pairs = 0;
            int i = 1;
            while (i <= 6)
            {
                if (counts[i] == 2) pairs += 1;
                i += 1;
            }
            return pairs;
        }
    }
}
