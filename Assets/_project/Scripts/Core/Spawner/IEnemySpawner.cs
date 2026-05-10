using _project.Scripts.Core.Enemy;
using _project.Scripts.Core.Pathfinding;
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

        public EnemySpawner(EnemyFactory factory, IPathService pathService, EnemyContainer container)
        {
            _factory = factory;
            _pathService = pathService;
            _enemyContainer = container;
        }

        public void SpawnOne()
        {
            var enemy = _factory.CreateEnemy();
            var path = _pathService.GetCurrentPath();

            enemy.GetBehavior<UnityMovement>().SetPath(path);
            enemy.GetBehavior<UnityMovement>().Move();

            _enemyContainer.AddEnemy(enemy);
        }
    }

}