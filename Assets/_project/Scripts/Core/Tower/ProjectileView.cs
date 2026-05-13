using _project.Scripts.Core.Enemy;
using _project.Scripts.Domain.Entitties;
using _project.Scripts.Domain.Interfaces;
using UnityEngine;

namespace _project.Scripts.Core.Tower
{
    public class ProjectileView : MonoBehaviour
    {
        [SerializeField] private float defaultSpeed = 12f;
        [SerializeField] private float hitRadius = 0.2f;
        [SerializeField] private float maxLifeTime = 5f;

        private Transform targetTransform;
        private IHealth targetHealth;
        private IAttackable targetAttackable;
        private float damage;
        private float speed;
        private float lifeTimer;

        public void Init(Vector3 origin, Domain.Entitties.Enemy target, float damage, float speed, float hitRadius)
        {
            transform.position = origin;
            this.damage = damage;
            this.speed = speed > 0f ? speed : defaultSpeed;
            this.hitRadius = hitRadius > 0f ? hitRadius : this.hitRadius;
            this.lifeTimer = 0f;

            targetHealth = target.GetBehavior<IHealth>();
            targetAttackable = target.GetBehavior<IAttackable>();

            if (target.GetEnemyView() is EnemyView view)
                targetTransform = view.transform;
        }

        private void Update()
        {
            lifeTimer += Time.deltaTime;
            if (lifeTimer >= maxLifeTime)
            {
                Destroy(gameObject);
                return;
            }

            if (targetTransform == null || targetHealth == null || !targetHealth.IsAlive)
            {
                Destroy(gameObject);
                return;
            }

            Vector3 targetPos = targetTransform.position;
            Vector3 toTarget = targetPos - transform.position;

            transform.rotation = Quaternion.LookRotation(toTarget.sqrMagnitude > 0.0001f ? toTarget : transform.forward);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

            if ((transform.position - targetPos).sqrMagnitude <= hitRadius * hitRadius)
            {
                targetAttackable?.ReceiveDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}
