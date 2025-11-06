using System;

namespace Dungeon
{
    public class EnemySpawner
    {
        private static bool _bossSpawnedThisRun = false;

        public static Enemy SpawnForRoom(Room room, Random rng)
        {
            if (room is BossEncounterRoom)
            {
                _bossSpawnedThisRun = true;
                return SpawnBoss(rng);
            }
            else
            {
                return SpawnNormal(rng);
            }
        }
        public static Enemy SpawnRandomNonBoss(Random rng)
        {
            return SpawnNormal(rng);
        }
        private static Enemy SpawnBoss(Random rng)
        {
            return new IronGolem(rng);
        }
        private static Enemy SpawnNormal(Random rng)
        {
            int roll = rng.Next(1, 11); // 1..10
            if (roll <= 3)
            {
                return new ArmouredOrc(rng);
            }
            else if (roll <= 6)
            {
                return new Goblin(rng);
            }
            else
            {
                return new CaveSlime(rng);
            }
        }
    }
}
