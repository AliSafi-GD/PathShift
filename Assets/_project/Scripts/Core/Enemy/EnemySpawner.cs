using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using _project.Scripts.Core.Pathfinding.Main;
using _project.Scripts.Domain.Grid;
using UnityEngine;

namespace _project.Scripts.Core.Enemy
{
    public class EnemySpawner : MonoBehaviour, IEnemySpawner
    {
        [Header("Config")]
        [SerializeField] private EnemySpawnConfig config;
        [SerializeField] private int spawnPointGridIndex;
        [SerializeField] private int endPointGridIndex;

        [Header("References")]
        [SerializeField] private EnemyPool enemyPool;

        public event Action<int> OnWaveStarted;
        public event Action<int> OnWaveEnded;
        public event Action OnAllWavesCompleted;

        public bool IsSpawning { get; private set; }
        public int CurrentWave { get; private set; }

        private IPathfinder _pathfinder;
        private IGrid _grid;
        private List<Vector3> _currentPath = new();
        private readonly List<EnemyView> _activeEnemies = new();
        private CancellationTokenSource _cts;
        private Coroutine _waveCoroutine;

        public void Construct(IPathfinder pathfinder, IGrid grid)
        {
            _pathfinder = pathfinder;
            _grid = grid;
        }

        private void OnDestroy() => StopSpawning();

        public void StartSpawning(CancellationToken cancellationToken = default)
        {
            if (IsSpawning)
            {
                Debug.LogWarning("[EnemySpawner] Already spawning.");
                return;
            }

            if (!ValidateSetup()) return;

            BuildPath();

            if (_currentPath.Count == 0)
            {
                Debug.LogError("[EnemySpawner] No path found between spawn and end point.");
                return;
            }

            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            IsSpawning = true;
            CurrentWave = 0;
            _waveCoroutine = StartCoroutine(RunWavesCoroutine(_cts.Token));
        }

        public void StopSpawning()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;

            if (_waveCoroutine != null)
            {
                StopCoroutine(_waveCoroutine);
                _waveCoroutine = null;
            }

            IsSpawning = false;
            Debug.Log("[EnemySpawner] Spawning stopped.");
        }

        public void RecalculatePaths()
        {
            if (!ValidateSetup()) return;

            BuildPath();

            if (_currentPath.Count == 0)
            {
                Debug.LogWarning("[EnemySpawner] RecalculatePaths: no path found.");
                return;
            }

            foreach (EnemyView enemy in _activeEnemies)
            {
                if (enemy != null && enemy.gameObject.activeSelf)
                    enemy.Move(new List<Vector3>(_currentPath));
            }
        }

        private IEnumerator RunWavesCoroutine(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                CurrentWave++;
                OnWaveStarted?.Invoke(CurrentWave);
                Debug.Log($"[EnemySpawner] Wave {CurrentWave} started.");

                for (int i = 0; i < config.enemiesPerWave; i++)
                {
                    if (token.IsCancellationRequested) yield break;
                    SpawnOneEnemy();
                    yield return new WaitForSeconds(config.spawnInterval);
                }

                OnWaveEnded?.Invoke(CurrentWave);
                Debug.Log($"[EnemySpawner] Wave {CurrentWave} ended.");

                yield return new WaitForSeconds(config.waveInterval);
            }

            IsSpawning = false;
            OnAllWavesCompleted?.Invoke();
            Debug.Log("[EnemySpawner] All waves completed.");
        }

        private void SpawnOneEnemy()
        {
            Vector3 spawnWorldPos = _currentPath[0];
            EnemyView enemy = enemyPool.Get(spawnWorldPos);
            _activeEnemies.Add(enemy);

            enemy.Move(new List<Vector3>(_currentPath));
            enemy.OnReachedEnd += () => ReturnEnemy(enemy);

            Debug.Log($"[EnemySpawner] Spawned enemy at {spawnWorldPos}");
        }

        private void ReturnEnemy(EnemyView enemy)
        {
            enemy.OnReachedEnd = null;
            _activeEnemies.Remove(enemy);
            enemyPool.Return(enemy);
        }

        private void BuildPath()
        {
            List<GridCell> cells = _grid.GetAllCells();
            int total = cells.Count;

            int startIdx = Mathf.Clamp(spawnPointGridIndex, 0, total - 1);
            int endIdx   = Mathf.Clamp(endPointGridIndex,   0, total - 1);

            PathCell startCell  = new PathCell(cells[startIdx].Position.X, cells[startIdx].Position.Y);
            PathCell endCell    = new PathCell(cells[endIdx].Position.X,   cells[endIdx].Position.Y);
            List<PathCell> walkable = cells.Select(c => new PathCell(c.Position.X, c.Position.Y)).ToList();

            List<PathCell> pathCells = _pathfinder.FindPath(startCell, endCell, walkable);

            _currentPath.Clear();
            foreach (PathCell pc in pathCells)
                _currentPath.Add(new Vector3(pc.X, 1f, pc.Y));
        }

        private bool ValidateSetup()
        {
            if (config == null)             { Debug.LogError("[EnemySpawner] config is null.");       return false; }
            if (config.enemyPrefab == null) { Debug.LogError("[EnemySpawner] enemyPrefab is null.");  return false; }
            if (_pathfinder == null)        { Debug.LogError("[EnemySpawner] _pathfinder not set."); return false; }
            if (_grid == null)              { Debug.LogError("[EnemySpawner] _grid not set.");        return false; }
            if (enemyPool == null)          { Debug.LogError("[EnemySpawner] enemyPool is null.");    return false; }
            return true;
        }
    }
}
