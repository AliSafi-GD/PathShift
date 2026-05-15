using System.Collections.Generic;
using _project.Scripts.Domain.Entities;
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
        public float Range { get; }

        public Tower(
            Vector3 position,
            float fireRate,
            float range,
            ITargetingPolicy targetingPolicy,
            IWeapon weapon)
        {
            Position = position;
            FireRate = fireRate;
            Range = range;
            this.targetingPolicy = targetingPolicy;
            this.weapon = weapon;
        }

        public void Tick(float deltaTime, IReadOnlyList<Domain.Entities.Enemy> enemies)
        {
            cooldownTimer -= deltaTime;
            if (cooldownTimer > 0f) return;

            var target = targetingPolicy.SelectTarget(Position, Range, enemies);
            if (target == null) return;

            weapon.Fire(Position, target);
            cooldownTimer = 1f / FireRate;
        }
    }
}
