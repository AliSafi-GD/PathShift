using System.Collections.Generic;
using _project.Scripts.Core.Enemy;
using UnityEngine;
using VContainer;

namespace _project.Scripts.Core.Tower
{
    public class TowerAttackSystem : MonoBehaviour
    {
        private readonly List<Tower> towers = new List<Tower>();
        private EnemyContainer enemyContainer;

        [Inject]
        public void Construct(EnemyContainer enemyContainer)
        {
            this.enemyContainer = enemyContainer;
        }

        public void Register(Tower tower)
        {
            if (tower == null) return;
            towers.Add(tower);
        }

        public void Unregister(Tower tower)
        {
            if (tower == null) return;
            towers.Remove(tower);
        }

        private void Update()
        {
            if (enemyContainer == null) return;
            if (towers.Count == 0) return;

            float dt = Time.deltaTime;
            var alive = enemyContainer.GetAliveEnemies();
            if (alive.Count == 0) return;

            for (int i = 0; i < towers.Count; i++)
                towers[i].Tick(dt, alive);
        }
    }
}
