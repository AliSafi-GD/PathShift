using _project.Scripts.Domain.Interfaces;
using UnityEngine;

namespace _project.Scripts.Presentation.View
{
    /// <summary>
    /// Melee: damages target on contact. Trigger Attack() from collision/trigger callback.
    /// </summary>
    public class UnityMeleeAttacker : MonoBehaviour, IAttacker
    {
        [SerializeField] private float damage = 10f;
        [SerializeField] private float attackInterval = 1f;

        private float cooldown;

        private void Update()
        {
            if (cooldown > 0f) cooldown -= Time.deltaTime;
        }

        public void Attack(IAttackable target)
        {
            if (target == null) return;
            if (cooldown > 0f) return;

            target.ReceiveDamage(damage);
            cooldown = attackInterval;
        }
    }

    // /// <summary>
    // /// Ranged: damages target if within range. Caller passes the target;
    // /// distance check uses target's transform if available.
    // /// </summary>
    // public class UnityRangedAttacker : MonoBehaviour, IAttacker
    // {
    //     [SerializeField] private int damage = 5;
    //     [SerializeField] private float range = 5f;
    //     [SerializeField] private float attackInterval = 1.5f;
    //
    //     private float cooldown;
    //
    //     private void Update()
    //     {
    //         if (cooldown > 0f) cooldown -= Time.deltaTime;
    //     }
    //
    //     public void Attack(IAttackable target)
    //     {
    //         if (target == null) return;
    //         if (cooldown > 0f) return;
    //
    //         // target is IAttackable; if it's a MonoBehaviour we can check distance
    //         if (target is MonoBehaviour mb)
    //         {
    //             float dist = Vector3.Distance(transform.position, mb.transform.position);
    //             if (dist > range) return;
    //         }
    //
    //         target.ReceiveDamage(damage);
    //         cooldown = attackInterval;
    //     }
    // }
}