using System.Collections.Generic;
using _project.Scripts.Core.Enemy;
using UnityEngine;

namespace _project.Scripts.Core.Tower
{
    public class Tower
    {
        public int Id;
        private readonly ITargetingPolicy targetingPolicy;
        private readonly IWeapon weapon;

        private float cooldownTimer;

        public Vector3 Position { get; }
        public float FireRate { get; }

        public Tower(
            Vector3 position,
            float fireRate,
            ITargetingPolicy targetingPolicy,
            IWeapon weapon)
        {
            Position = position;
            FireRate = fireRate;
            this.targetingPolicy = targetingPolicy;
            this.weapon = weapon;
        }

        public void Tick(float deltaTime, IReadOnlyList<EnemyView> enemies)
        {
            cooldownTimer -= deltaTime;
            if (cooldownTimer > 0) return;

            var target = targetingPolicy.SelectTarget(Position, enemies);
            if (target == null) return;

            weapon.Fire(Position, target);

            cooldownTimer = 1f / FireRate;
        }
    }

}