using _project.Scripts.Presentation.View;
using Unity.VisualScripting;
using UnityEngine;

namespace _project.Scripts.Core.Enemy
{
    public interface IEnemyFactory
    {
        Domain.Entities.Enemy CreateEnemy(Vector3 spawnPosition, EnemyConfig config);
    }

    public class EnemyFactory : MonoBehaviour, IEnemyFactory
    {
        [SerializeField] private EnemyView fallbackPrefab;

        public Domain.Entities.Enemy CreateEnemy(Vector3 spawnPosition, EnemyConfig config)
        {
            var prefab = (config != null && config.Prefab != null) ? config.Prefab : fallbackPrefab;
            EnemyView instance = Instantiate(prefab, spawnPosition, Quaternion.identity);

            // Prepare spawn animation BEFORE the first render so the scale doesn't pop.
            var spawnAnimator = instance.GetOrAddComponent<EnemySpawnAnimator>();
            spawnAnimator.Prepare();
            instance.GetOrAddComponent<EnemyWalkAnimator>();
            instance.GetOrAddComponent<EnemyDeathAnimator>();

            var unityMovement = instance.GetOrAddComponent<UnityMovement>();
            var unityHealth = instance.GetOrAddComponent<UnityHealth>();
            var unityAttacker = instance.GetOrAddComponent<UnityMeleeAttacker>();
            var unityAttackable = instance.GetOrAddComponent<UnityAttackable>();

            if (config != null)
            {
                unityHealth.SetMaxHealth(config.MaxHealth);
                unityMovement.SetSpeed(config.MoveSpeed);
                unityAttacker.SetStats(config.Damage, config.AttackInterval);
            }

            return new Domain.Entities.Enemy(unityMovement, unityHealth, unityAttackable, unityAttacker, instance);
        }
    }
}
