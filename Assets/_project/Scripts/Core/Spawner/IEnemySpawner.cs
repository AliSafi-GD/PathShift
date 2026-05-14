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

            var unityMovement = enemy.GetBehavior<UnityMovement>();
            unityMovement.SetPath(path);

            // انیمیشن spawn رو اجرا کن؛ بعد از پایان، حرکت شروع بشه.
            var view = enemy.GetEnemyView() as EnemyView;
            var anim = view != null ? view.GetComponent<EnemySpawnAnimator>() : null;
            if (anim != null)
                anim.Play().OnComplete(() => unityMovement.Move());
            else
                unityMovement.Move();

            unityMovement.OnFinishedMove += () =>
            {
                var attacker = enemy.GetBehavior<IAttacker>();
                var attackable = mainTower?.GetBehavior<IAttackable>();

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

                var selfHealth = enemy.GetBehavior<IHealth>();
                if (selfHealth != null && selfHealth.IsAlive)
                {
                    reachedEnd = true;
                    selfHealth.TakeDamage(selfHealth.MaxHealth);
                }
            };

            var health = enemy.GetBehavior<IHealth>();
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

                    if (enemy.GetEnemyView() is EnemyView view && view != null)
                        Object.Destroy(view.gameObject);
                };
                health.OnDied += onDied;
            }

            _enemyContainer.AddEnemy(enemy);
            OnEnemySpawned?.Invoke(config);
        }
    }
}
