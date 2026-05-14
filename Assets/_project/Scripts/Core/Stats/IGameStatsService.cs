using System;

namespace _project.Scripts.Core.Stats
{
    public interface IGameStatsService
    {
        // Wave
        int TotalWaves { get; }
        int CurrentWave { get; }      // 0 قبل از شروع، 1..N

        // Enemies
        int TotalEnemiesInLevel { get; }
        int EnemiesSpawned { get; }
        int EnemiesKilledByTower { get; }
        int EnemiesReachedEnd { get; }
        int EnemiesAlive => EnemiesSpawned - EnemiesKilledByTower - EnemiesReachedEnd;
        int EnemiesRemainingToSpawn => System.Math.Max(0, TotalEnemiesInLevel - EnemiesSpawned);

        event Action StatsChanged;
    }
}
