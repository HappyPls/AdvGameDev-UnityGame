using UnityEngine;

namespace Dungeon
{
    [CreateAssetMenu(menuName = "Dungeon/Grid Settings", fileName = "GridSettings")]
    public class GridSettings : ScriptableObject
    {
        [Header("Grid Size")]
        [Min(3)] public int Rows = 10;
        [Min(3)] public int Cols = 10;

        [Header("Room Placement Minimums")]
        [Min(0)] public int MinTreasure = 2;
        [Min(0)] public int MinEncounter = 5;
        [Min(0)] public int MinBoss = 1;
        [Min(0)] public int MinTrap = 3;
        [Min(0)] public int MinSafe = 2;

        [Header("Spawn / Visual")]
        public float CellSize = 1f;
        public float RoomScale = 1f;
        public bool KeepBossFarFromCenter = true;
        public int BossMinManhattanDistance = 3;

        [Header("Seeding")]
        public bool UseFixedSeed = false;
        public int Seed = 12345;
    }
}
