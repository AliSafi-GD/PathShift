using _project.Scripts.Core.Enemy;
using _project.Scripts.Domain.Interfaces;
using DG.Tweening;
using UnityEngine;

namespace _project.Scripts.Core.Tower
{
    // پرتابه قوسی mortar. به نقطه‌ی هدف می‌ره و موقع برخورد به همه enemy‌های داخل splashRadius آسیب می‌زنه.
    public class MortarProjectileView : MonoBehaviour
    {
        private float damage;
        private float splashRadius;
        private EnemyContainer container;

        public void Init(Vector3 origin, Vector3 target, float damage,
                         float splashRadius, float arcHeight, float duration,
                         EnemyContainer container)
        {
            transform.position = origin;
            this.damage = damage;
            this.splashRadius = splashRadius;
            this.container = container;

            // قوس به نقطه target
            transform.DOJump(target, arcHeight, 1, Mathf.Max(0.1f, duration))
                .SetEase(Ease.Linear)
                .OnComplete(Explode);
        }

        private void Explode()
        {
            if (container != null)
            {
                var alive = container.GetAliveEnemies();
                float r2 = splashRadius * splashRadius;
                var pos = transform.position;

                for (int i = 0; i < alive.Count; i++)
                {
                    var enemy = alive[i];
                    if (!(enemy.View is EnemyView view) || view == null) continue;
                    if ((view.transform.position - pos).sqrMagnitude > r2) continue;

                    enemy.Attackable?.ReceiveDamage(damage);
                }
            }

            Destroy(gameObject);
        }

        private void OnDestroy() => transform.DOKill();
    }
}
