using System;
using System.Threading;

namespace _project.Scripts.Core.Enemy
{
    public interface IEnemySpawner
    {
        event Action<int> OnWaveStarted;
        event Action<int> OnWaveEnded;
        event Action OnAllWavesCompleted;

        bool IsSpawning { get; }
        int CurrentWave { get; }

        void StartSpawning(CancellationToken cancellationToken = default);
        void StopSpawning();
        void RecalculatePaths();
    }
}
