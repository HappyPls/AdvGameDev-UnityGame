using UnityEngine;

namespace Dungeon
{
    [System.Serializable]
    public class Dice
    {
        [Tooltip("Number of sides on this die (e.g., 6 for a d6)")]
        public int Sides = 6;

        [Tooltip("Die Name")]
        public string Label = "";

        public Dice() { }
        public Dice(int sides, string label = "")
        {
            Sides = Mathf.Max(2, sides);
            Label = label;
        }

        public int Roll(System.Random rng)
        {
            if (rng == null) rng = new System.Random();
            return rng.Next(1, Sides + 1);
        }
    }
}
