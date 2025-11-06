using System;

namespace Dungeon
{
    public class EnemySpawner
    {
        private static bool _golemSpawned = false;
        private static bool _forceGolem = false;

        public static void ForceGolemSpawn()
        {
            _forceGolem = true;
        }

        public static Enemy SpawnRandomEnemy(Random rng)
        {
            if (_forceGolem && !_golemSpawned)
            {
                _forceGolem = false;
                _golemSpawned = true;
                return new IronGolem(rng);
            }

            int roll = rng.Next(1, 11);

            if (!_golemSpawned && roll == 1)
            {
                _golemSpawned = true;
                return new IronGolem(rng);
            }
            else if (roll <= 3)
                return new ArmouredOrc(rng);
            else if (roll <= 6)
                return new Goblin(rng);
            else
                return new CaveSlime(rng);
        }
    }
}
