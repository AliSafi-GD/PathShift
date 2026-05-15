using _project.Scripts.Presentation.View;
using Unity.VisualScripting;
using UnityEngine;

namespace _project.Scripts.Core.Enemy
{
    public class EnemyFactory : MonoBehaviour
    {
        [SerializeField] private EnemyView fallbackPrefab;

        private Transform startSpawnTransform;

        public void Setup(Transform startSpawnTransform)
        {
            this.startSpawnTransform = startSpawnTransform;
        }

        public Domain.Entities.Enemy CreateEnemy(Vector3 spawnPosition, EnemyConfig config)
        {
            var prefab = (config != null && config.Prefab != null) ? config.Prefab : fallbackPrefab;
            EnemyView instance = Instantiate(prefab, spawnPosition, Quaternion.identity);

            // قبل از render اول، اگه spawn animator هست، scale رو صفر کن تا pop نشه.
            var spawnAnimator = instance.GetOrAddComponent<EnemySpawnAnimator>();
            spawnAnimator.Prepare();
            // walk anim (روی child visual). اگه child نباشه خود animator skip می‌کنه.
            instance.GetOrAddComponent<EnemyWalkAnimator>();
            // death anim (spawner از این برای تأخیر Destroy استفاده می‌کنه).
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
