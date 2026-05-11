using _project.Scripts.Core.Enemy;
using _project.Scripts.Core.Map;
using _project.Scripts.Core.Pathfinding;
using _project.Scripts.Core.Tower;
using _project.Scripts.Domain.Interfaces;
using _project.Scripts.Presentation.View;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _project.Scripts.Core.Spawner
{
    public interface IEnemySpawner
    {
        void SpawnOne();
    }
    public class EnemySpawner : IEnemySpawner
    {
        private readonly EnemyFactory _factory;
        private readonly IPathService _pathService;
        private readonly EnemyContainer _enemyContainer;
        private readonly MainTower mainTower;

        public EnemySpawner(EnemyFactory factory, IPathService pathService, EnemyContainer container,MainTower mainTower)
        {
            _factory = factory;
            _pathService = pathService;
            _enemyContainer = container;
            this.mainTower = mainTower;
        }

        public void SpawnOne()
        {
            var enemy = _factory.CreateEnemy(_pathService.GetCurrentPath()[0].WorldPosition);
            var path = _pathService.GetCurrentPath();

            var unityMovement = enemy.GetBehavior<UnityMovement>();
            unityMovement.SetPath(path);
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
                    selfHealth.TakeDamage(selfHealth.MaxHealth);
            };

            var health = enemy.GetBehavior<IHealth>();
            if (health != null)
            {
                System.Action onDied = null;
                onDied = () =>
                {
                    health.OnDied -= onDied;
                    _enemyContainer.RemoveItem(enemy);

                    if (enemy.GetEnemyView() is EnemyView view && view != null)
                        Object.Destroy(view.gameObject);
                };
                health.OnDied += onDied;
            }

            _enemyContainer.AddEnemy(enemy);
        }
    }

}