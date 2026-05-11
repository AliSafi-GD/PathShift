using _project.Scripts.Core.Enemy;
using _project.Scripts.Core.Map;
using _project.Scripts.Core.Pathfinding;
using _project.Scripts.Core.Tower;
using _project.Scripts.Domain.Interfaces;
using _project.Scripts.Presentation.View;

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
                var en = enemy;
                en.GetBehavior<IAttacker>().Attack(mainTower.GetBehavior<IAttackable>());
            };
            _enemyContainer.AddEnemy(enemy);
        }
    }

}