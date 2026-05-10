using _project.Scripts.Core.Enemy;
using UnityEngine;

namespace _project.Scripts.Core.Tower
{
    public class ProjectileFactory : MonoBehaviour,IProjectileFactory
    {
        [SerializeField] private ProjectileView prefab;
        public void Create(Vector3 origin, EnemyView target, float damage)
        {
            var projectileGO = Instantiate(prefab);
            // projectileGO.GetComponent<ProjectileView>()
            //     .Init(origin+Vector3.up, target, damage);
        }
    }
}