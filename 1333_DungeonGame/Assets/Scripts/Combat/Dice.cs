using UnityEngine;

namespace Dungeon
{
    [CreateAssetMenu(fileName = "Dice", menuName = "Dungeon/Dice")]
    public class Dice : ScriptableObject
    {
        [Header("Dice Settings")]
        [Min(2)]
        public int Sides = 6;

        public int Roll(System.Random rng)
        {
            if (rng == null)
            {
                return Random.Range(1, Sides + 1);
            }

            return rng.Next(1, Sides + 1);
        }

        public int RollUnity()
        {
            return Random.Range(1, Sides + 1);
        }
    }
}
