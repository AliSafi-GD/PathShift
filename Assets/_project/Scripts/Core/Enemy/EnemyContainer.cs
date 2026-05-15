using System.Collections.Generic;
using System.Linq;

namespace _project.Scripts.Core.Enemy
{
    public class EnemyContainer
    {
        private readonly List<Domain.Entities.Enemy> enemies = new List<Domain.Entities.Enemy>();

        public void AddEnemy(Domain.Entities.Enemy enemy)
        {
            enemies.Add(enemy);
        }

        public void RemoveDead()
        {
            enemies.RemoveAll(e => !e.Health.IsAlive);
        }

        public void RemoveItem(Domain.Entities.Enemy enemyView)
        {
            enemies.Remove(enemyView);
        }

        public List<Domain.Entities.Enemy> GetAliveEnemies()
        {
            return enemies.Where(e => e.Health.IsAlive).ToList();
        }
    }
}
