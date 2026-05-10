using _project.Scripts.Domain.Interfaces;
using UnityEngine;

namespace _project.Scripts.Presentation.View
{
    public class UnityAttackable : MonoBehaviour ,IAttackable
    {
        UnityHealth health;
        public void ReceiveDamage(int damage)
        {
            health.CurrentHealth -= damage;
        }
    }
}