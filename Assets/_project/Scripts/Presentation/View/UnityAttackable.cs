using _project.Scripts.Domain.Interfaces;
using Unity.VisualScripting;
using UnityEngine;

namespace _project.Scripts.Presentation.View
{
    [RequireComponent(typeof(UnityHealth))]
    public class UnityAttackable : MonoBehaviour, IAttackable
    {
        private UnityHealth health;

        private void Awake()
        {
            health = this.GetOrAddComponent<UnityHealth>();
        }

        public void ReceiveDamage(int damage)
        {
            if (health == null) return;
            health.TakeDamage(damage);
            Debug.Log("received damage: " + damage);
        }
    }
}