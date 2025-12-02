using UnityEngine;

namespace Dungeon
{
    public enum CombatState
    {
        None,
        Intro,
        PlayerTurn,
        EnemyTurn,
        Resolving,
        Victory,
        Defeat,
        Finished
    }
}
