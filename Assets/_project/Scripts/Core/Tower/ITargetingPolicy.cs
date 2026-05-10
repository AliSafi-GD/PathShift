using System.Collections.Generic;
using _project.Scripts.Core.Enemy;
using UnityEngine;

namespace _project.Scripts.Core.Tower
{
   

    public interface IProjectileFactory
    {
        void Create(Vector3 origin, EnemyView target, float damage);
    }

    public class CannonWeapon : IWeapon
    {
        private readonly IProjectileFactory projectileFactory;
        private readonly float damage;

        public CannonWeapon(IProjectileFactory projectileFactory, float damage)
        {
            this.projectileFactory = projectileFactory;
            this.damage = damage;
        }

        public void Fire(Vector3 origin, EnemyView target)
        {
            projectileFactory.Create(origin, target, damage);
        }
    }

    public interface IWeapon
    {
        void Fire(Vector3 origin, EnemyView target);
    }

    public class ClosestTargetPolicy : ITargetingPolicy
    {
        public EnemyView SelectTarget(Vector3 towerPosition, IReadOnlyList<EnemyView> enemies)
        {
            EnemyView best = null;
            float minDistance = 2;

            foreach (var enemy in enemies)
            {
                // if (!enemy.IsAlive) continue;

                float dist = Vector3.Distance(towerPosition, enemy.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    best = enemy;
                }
            }
            return best;
        }
    }

    public interface ITargetingPolicy
    {
        EnemyView SelectTarget(Vector3 towerPosition, IReadOnlyList<EnemyView> enemies);
    }

}