using _project.Scripts.Core.Enemy;
using UnityEngine;

namespace _project.Scripts.Core.Tower
{
    public class ProjectileView : MonoBehaviour
    {
        private Transform target;
        private float damage;
        public float speed = 10f;
        // IDamageable damageable;
        // IHealth health;

        // public void Init(Vector3 origin, IDamageable target, float damage)
        // {
        //     transform.position = origin;
        //     damageable = target;
        //     this.target = ((MonoBehaviour)target).transform;
        //     this.damage = damage;
        // }

        private void Update()
        {
            // if (target == null || !health.IsAlive)
            // {
            //     Destroy(gameObject);
            //     return;
            // }
            //
            // transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position);
            // transform.position = Vector3.MoveTowards(
            //     transform.position,
            //     target.transform.position,
            //     speed * Time.deltaTime
            // );
            //
            // if (Vector3.Distance(transform.position, target.transform.position) < 0.2f)
            // {
            //     damageable.TakeDamage(damage);
            //     Destroy(gameObject);
            // }
        }
    }

}