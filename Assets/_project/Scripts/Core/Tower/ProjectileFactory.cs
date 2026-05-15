using _project.Scripts.Domain.Entities;
using UnityEngine;

namespace _project.Scripts.Core.Tower
{
    public class ProjectileFactory : MonoBehaviour, IProjectileFactory
    {
        [SerializeField] private ProjectileView prefab;
        [SerializeField] private float projectileSpeed = 12f;
        [SerializeField] private float hitRadius = 0.2f;

        public void Create(Vector3 origin, Domain.Entities.Enemy target, float damage)
        {
            if (prefab == null || target == null) return;

            var instance = Instantiate(prefab);
            instance.Init(origin + Vector3.up, target, damage, projectileSpeed, hitRadius);
        }
    }
}
