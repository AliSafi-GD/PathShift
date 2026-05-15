using _project.Scripts.Core.Economy;
using _project.Scripts.Core.Enemy;
using _project.Scripts.Core.Pathfinding;
using _project.Scripts.Core.Tower;
using _project.Scripts.Domain.Interfaces;
using _project.Scripts.Presentation.View;
using DG.Tweening;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _project.Scripts.Core.Spawner
{
    public interface IEnemySpawner
    {
        event System.Action<EnemyConfig> OnEnemySpawned;
        // reachedEnd = true یعنی به base رسیده (kill محسوب نمیشه برای آمار/پاداش)
        event System.Action<EnemyConfig, bool> OnEnemyDied;

        void Spawn(EnemyConfig config);
    }

    public class EnemySpawner : IEnemySpawner
    {
        private readonly EnemyFactory _factory;
        private readonly IPathService _pathService;
        private readonly EnemyContainer _enemyContainer;
        private readonly MainTower mainTower;
        private readonly IWallet wallet;

        public event System.Action<EnemyConfig> OnEnemySpawned;
        public event System.Action<EnemyConfig, bool> OnEnemyDied;

        public EnemySpawner(
            EnemyFactory factory,
            IPathService pathService,
            EnemyContainer container,
            MainTower mainTower,
            IWallet wallet)
        {
            _factory = factory;
            _pathService = pathService;
            _enemyContainer = container;
            this.mainTower = mainTower;
            this.wallet = wallet;
        }

        public void Spawn(EnemyConfig config)
        {
            if (config == null)
            {
                Debug.LogError("EnemySpawner.Spawn called with null EnemyConfig.");
                return;
            }

            var enemy = _factory.CreateEnemy(_pathService.GetCurrentPath()[0].WorldPosition, config);
            var path = _pathService.GetCurrentPath();

            bool reachedEnd = false;

            var movement = enemy.Movement;
            movement.SetPath(path);

            // انیمیشن spawn رو اجرا کن؛ بعد از پایان، حرکت شروع بشه.
            var view = enemy.View as EnemyView;
            var anim = view != null ? view.GetComponent<EnemySpawnAnimator>() : null;
            if (anim != null)
                anim.Play().OnComplete(() => movement.Move());
            else
                movement.Move();

            movement.OnFinishedMove += () =>
            {
                var attacker = enemy.Attacker;
                var attackable = mainTower?.Attackable;

                if (attacker == null)
                {
                    Debug.LogError("Enemy has no IAttacker behavior!");
                    return;
                }
                if (attackable == null)
                {
                    Debug.LogError("MainTower has no IAttackable behavior!");
                    return;
                }

                attacker.Attack(attackable);

                var selfHealth = enemy.Health;
                if (selfHealth != null && selfHealth.IsAlive)
                {
                    reachedEnd = true;
                    selfHealth.TakeDamage(selfHealth.MaxHealth);
                }
            };

            var health = enemy.Health;
            if (health != null)
            {
                System.Action onDied = null;
                onDied = () =>
                {
                    health.OnDied -= onDied;
                    _enemyContainer.RemoveItem(enemy);

                    if (!reachedEnd && wallet != null && !config.KillReward.IsFree)
                        wallet.Add(config.KillReward.Type, config.KillReward.Amount);

                    OnEnemyDied?.Invoke(config, reachedEnd);

                    var dyingView = enemy.View as EnemyView;
                    if (dyingView != null)
                    {
                        var go = dyingView.gameObject;
                        // اگه به base رسیده بدون انیمیشن حذف بشه (سریع).
                        // وگرنه (مرگ توسط tower) با انیمیشن خداحافظی.
                        var deathAnim = go.GetComponent<EnemyDeathAnimator>();
                        if (!reachedEnd && deathAnim != null)
                            deathAnim.Play(() => { if (go != null) Object.Destroy(go); });
                        else
                            Object.Destroy(go);
                    }
                };
                health.OnDied += onDied;
            }

            _enemyContainer.AddEnemy(enemy);
            OnEnemySpawned?.Invoke(config);
        }
    }
}
