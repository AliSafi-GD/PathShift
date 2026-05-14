using System;
using System.Threading;
using System.Threading.Tasks;
using _project.Scripts.Core.Enemy;
using IEnemySpawner = _project.Scripts.Core.Spawner.IEnemySpawner;

namespace _project.Scripts.Core.Wave
{
    public interface IWaveService
    {
        event Action<int> OnWaveStarted;
        event Action<int> OnWaveEnded;
        event Action OnAllWavesCompleted;

        Task Start();
        void Stop();
    }

    public class WaveService : IWaveService
    {
        private readonly LevelConfig level;
        private readonly IEnemySpawner spawner;
        private readonly EnemyContainer enemyContainer;

        public event Action<int> OnWaveStarted;
        public event Action<int> OnWaveEnded;
        public event Action OnAllWavesCompleted;

        private CancellationTokenSource cts;
        private int currentWave;

        public WaveService(LevelConfig level, IEnemySpawner spawner, EnemyContainer enemyContainer)
        {
            this.level = level;
            this.spawner = spawner;
            this.enemyContainer = enemyContainer;
        }

        public async Task Start()
        {
            if (level == null || level.Waves == null || level.Waves.Count == 0)
            {
                UnityEngine.Debug.LogWarning("WaveService: LevelConfig خالیه.");
                return;
            }

            cts = new CancellationTokenSource();
            currentWave = 0;

            try
            {
                if (level.StartDelay > 0f)
                    await Task.Delay(TimeSpan.FromSeconds(level.StartDelay), cts.Token);

                foreach (var wave in level.Waves)
                {
                    if (cts.IsCancellationRequested) return;
                    currentWave++;
                    OnWaveStarted?.Invoke(currentWave);

                    foreach (var group in wave.Groups)
                    {
                        if (cts.IsCancellationRequested) return;
                        if (group.delayBefore > 0f)
                            await Task.Delay(TimeSpan.FromSeconds(group.delayBefore), cts.Token);

                        for (int i = 0; i < group.count; i++)
                        {
                            if (cts.IsCancellationRequested) return;
                            spawner.Spawn(group.enemy);
                            if (group.interval > 0f && i < group.count - 1)
                                await Task.Delay(TimeSpan.FromSeconds(group.interval), cts.Token);
                        }
                    }

                    OnWaveEnded?.Invoke(currentWave);

                    if (wave.DelayAfter > 0f)
                        await Task.Delay(TimeSpan.FromSeconds(wave.DelayAfter), cts.Token);
                }

                // همه waves spawn شدن. حالا منتظر بمون تا همه‌ی enemyهای زنده هم تموم بشن.
                await WaitUntilNoEnemies(cts.Token);

                OnAllWavesCompleted?.Invoke();
            }
            catch (TaskCanceledException) { /* normal stop */ }
        }

        private async Task WaitUntilNoEnemies(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (enemyContainer == null || enemyContainer.GetAliveEnemies().Count == 0)
                    return;
                await Task.Delay(200, token);
            }
        }

        public void Stop()
        {
            cts?.Cancel();
        }

        ~WaveService()
        {
            Stop();
        }
    }
}
