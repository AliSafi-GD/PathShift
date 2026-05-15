using _project.Scripts.Core.Enemy;
using UnityEngine;
using VContainer;

namespace _project.Scripts.Core.Tower
{
    // Ticks every placed tower once per frame. Source of truth for which towers
    // exist is IPlacedTowerRegistry — this system never owns its own list.
    public class TowerAttackSystem : MonoBehaviour
    {
        private IPlacedTowerRegistry registry;
        private EnemyContainer enemyContainer;

        [Inject]
        public void Construct(IPlacedTowerRegistry registry, EnemyContainer enemyContainer)
        {
            this.registry = registry;
            this.enemyContainer = enemyContainer;
        }

        private void Update()
        {
            if (registry == null || enemyContainer == null) return;

            var all = registry.All;
            if (all.Count == 0) return;

            var alive = enemyContainer.GetAliveEnemies();
            if (alive.Count == 0) return;

            float dt = Time.deltaTime;
            for (int i = 0; i < all.Count; i++)
                all[i].Tower?.Tick(dt, alive);
        }
    }
}
