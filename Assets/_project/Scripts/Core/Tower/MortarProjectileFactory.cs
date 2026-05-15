using _project.Scripts.Core.Enemy;
using UnityEngine;
using VContainer;

namespace _project.Scripts.Core.Tower
{
    public interface IMortarProjectileFactory
    {
        void Create(Vector3 origin, Vector3 targetPosition, float damage,
                    float splashRadius, float arcHeight, float travelTime);
    }

    public class MortarProjectileFactory : MonoBehaviour, IMortarProjectileFactory
    {
        [SerializeField] private MortarProjectileView prefab;
        private EnemyContainer enemyContainer;

        [Inject]
        public void Construct(EnemyContainer enemyContainer)
        {
            this.enemyContainer = enemyContainer;
        }

        public void Create(Vector3 origin, Vector3 targetPosition, float damage,
                           float splashRadius, float arcHeight, float travelTime)
        {
            if (prefab == null) return;
            var p = Instantiate(prefab);
            p.Init(origin + Vector3.up, targetPosition, damage, splashRadius, arcHeight, travelTime, enemyContainer);
        }
    }
}
