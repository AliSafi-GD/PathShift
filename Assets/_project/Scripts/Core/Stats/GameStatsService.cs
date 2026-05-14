using System;
using _project.Scripts.Core.Enemy;
using _project.Scripts.Core.Spawner;
using _project.Scripts.Core.Wave;
using VContainer.Unity;

namespace _project.Scripts.Core.Stats
{
    // آمار بازی رو از روی event‌های wave/spawner جمع می‌کنه.
    // UI فقط به StatsChanged subscribe می‌کنه و مقادیر رو می‌خونه.
    public class GameStatsService : IGameStatsService, IStartable, IDisposable
    {
        private readonly LevelConfig level;
        private readonly IEnemySpawner spawner;
        private readonly IWaveService waveService;

        public int TotalWaves { get; private set; }
        public int CurrentWave { get; private set; }
        public int TotalEnemiesInLevel { get; private set; }
        public int EnemiesSpawned { get; private set; }
        public int EnemiesKilledByTower { get; private set; }
        public int EnemiesReachedEnd { get; private set; }

        public event Action StatsChanged;

        public GameStatsService(LevelConfig level, IEnemySpawner spawner, IWaveService waveService)
        {
            this.level = level;
            this.spawner = spawner;
            this.waveService = waveService;
        }

        public void Start()
        {
            // محاسبه‌ی totalها از روی LevelConfig
            if (level != null && level.Waves != null)
            {
                TotalWaves = level.Waves.Count;
                int total = 0;
                foreach (var w in level.Waves)
                {
                    if (w == null) continue;
                    foreach (var g in w.Groups) total += g.count;
                }
                TotalEnemiesInLevel = total;
            }

            if (waveService != null)
            {
                waveService.OnWaveStarted += HandleWaveStarted;
            }
            if (spawner != null)
            {
                spawner.OnEnemySpawned += HandleSpawned;
                spawner.OnEnemyDied += HandleDied;
            }

            Raise();
        }

        public void Dispose()
        {
            if (waveService != null) waveService.OnWaveStarted -= HandleWaveStarted;
            if (spawner != null)
            {
                spawner.OnEnemySpawned -= HandleSpawned;
                spawner.OnEnemyDied -= HandleDied;
            }
        }

        private void HandleWaveStarted(int wave) { CurrentWave = wave; Raise(); }
        private void HandleSpawned(EnemyConfig _) { EnemiesSpawned++; Raise(); }
        private void HandleDied(EnemyConfig _, bool reachedEnd)
        {
            if (reachedEnd) EnemiesReachedEnd++;
            else EnemiesKilledByTower++;
            Raise();
        }

        private void Raise() => StatsChanged?.Invoke();
    }
}
