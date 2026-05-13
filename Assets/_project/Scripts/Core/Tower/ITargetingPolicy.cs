using System.Collections.Generic;
using _project.Scripts.Core.Enemy;
using _project.Scripts.Domain.Entitties;
using _project.Scripts.Domain.Interfaces;
using UnityEngine;

namespace _project.Scripts.Core.Tower
{
    public interface IProjectileFactory
    {
        void Create(Vector3 origin, Domain.Entitties.Enemy target, float damage);
    }

    public interface IWeapon
    {
        void Fire(Vector3 origin, Domain.Entitties.Enemy target);
    }

    public interface ITargetingPolicy
    {
        Domain.Entitties.Enemy SelectTarget(Vector3 towerPosition, float range, IReadOnlyList<Domain.Entitties.Enemy> enemies);
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

        public void Fire(Vector3 origin, Domain.Entitties.Enemy target)
        {
            projectileFactory.Create(origin, target, damage);
        }
    }

    public class ClosestTargetPolicy : ITargetingPolicy
    {
        public Domain.Entitties.Enemy SelectTarget(Vector3 towerPosition, float range, IReadOnlyList<Domain.Entitties.Enemy> enemies)
        {
            Domain.Entitties.Enemy best = null;
            float minSqr = range * range;

            foreach (var enemy in enemies)
            {
                var health = enemy.GetBehavior<IHealth>();
                if (health == null || !health.IsAlive) continue;

                if (!(enemy.GetEnemyView() is EnemyView view)) continue;

                float sqr = (view.transform.position - towerPosition).sqrMagnitude;
                if (sqr <= minSqr)
                {
                    minSqr = sqr;
                    best = enemy;
                }
            }
            return best;
        }
    }
}
