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
        private readonly EnemySpawnConfig _config;
        private readonly IEnemySpawner _spawner;
    
        public event Action<int> OnWaveStarted;
        public event Action<int> OnWaveEnded;
        public event Action OnAllWavesCompleted;

        private CancellationTokenSource _cts;
        private int _currentWave;

        public WaveService(EnemySpawnConfig config, IEnemySpawner spawner)
        {
            _config = config;
            _spawner = spawner;
        }

        public async Task Start()
        {
            _cts = new CancellationTokenSource();
            _currentWave = 0;

            while (!_cts.IsCancellationRequested)
            {
                _currentWave++;
                OnWaveStarted?.Invoke(_currentWave);

                for (int i = 0; i < _config.enemiesPerWave; i++)
                {
                    _spawner.SpawnOne();
                    await Task.Delay(TimeSpan.FromSeconds(_config.spawnInterval), _cts.Token);
                }

                OnWaveEnded?.Invoke(_currentWave);
                await Task.Delay(TimeSpan.FromSeconds(_config.waveInterval), _cts.Token);
            }

            OnAllWavesCompleted?.Invoke();
        }

        public void Stop()
        {
            _cts?.Cancel();
        }
        ~WaveService()
        {
            Stop();
        }
    }
}