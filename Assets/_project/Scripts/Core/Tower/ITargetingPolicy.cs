using System.Collections.Generic;
using _project.Scripts.Core.Enemy;
using _project.Scripts.Domain.Entities;
using _project.Scripts.Domain.Interfaces;
using UnityEngine;

namespace _project.Scripts.Core.Tower
{
    public interface IProjectileFactory
    {
        void Create(Vector3 origin, Domain.Entities.Enemy target, float damage);
    }

    public interface IWeapon
    {
        void Fire(Vector3 origin, Domain.Entities.Enemy target);
    }

    public interface ITargetingPolicy
    {
        Domain.Entities.Enemy SelectTarget(Vector3 towerPosition, float range, IReadOnlyList<Domain.Entities.Enemy> enemies);
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

        public void Fire(Vector3 origin, Domain.Entities.Enemy target)
        {
            projectileFactory.Create(origin, target, damage);
        }
    }

    // پرتاب قوسی به مکان فعلی هدف، با AoE هنگام برخورد.
    public class MortarWeapon : IWeapon
    {
        private readonly MortarProjectileFactory factory;
        private readonly float damage;
        private readonly float splashRadius;
        private readonly float arcHeight;
        private readonly float travelTime;

        public MortarWeapon(MortarProjectileFactory factory, float damage,
                            float splashRadius, float arcHeight, float travelTime)
        {
            this.factory = factory;
            this.damage = damage;
            this.splashRadius = splashRadius;
            this.arcHeight = arcHeight;
            this.travelTime = travelTime;
        }

        public void Fire(Vector3 origin, Domain.Entities.Enemy target)
        {
            if (factory == null) return;
            if (!(target.GetEnemyView() is EnemyView view) || view == null) return;
            factory.Create(origin, view.transform.position, damage, splashRadius, arcHeight, travelTime);
        }
    }

    public class ClosestTargetPolicy : ITargetingPolicy
    {
        public Domain.Entities.Enemy SelectTarget(Vector3 towerPosition, float range, IReadOnlyList<Domain.Entities.Enemy> enemies)
        {
            Domain.Entities.Enemy best = null;
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
