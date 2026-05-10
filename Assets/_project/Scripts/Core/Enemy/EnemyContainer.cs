using System.Collections.Generic;
using System.Linq;
using _project.Scripts.Domain.Interfaces;

namespace _project.Scripts.Core.Enemy
{
    public class EnemyContainer
    {
        private readonly List<Domain.Entitties.Enemy> enemies = new List<Domain.Entitties.Enemy>();

        public void AddEnemy(Domain.Entitties.Enemy enemy)
        {
            enemies.Add(enemy);
        }
        public void RemoveDead()
        {
            enemies.RemoveAll(e => !e.GetBehavior<IHealth>().IsAlive);
        }

        public void RemoveItem(Domain.Entitties.Enemy enemyView)
        {
            enemies.Remove(enemyView);
        }

        public List<Domain.Entitties.Enemy> GetAliveEnemies()
        {
            return enemies.Where(e => e.GetBehavior<IHealth>().IsAlive).ToList();
        }
    }
}