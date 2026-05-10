using _project.Scripts.Domain.Interfaces;
using UnityEngine;

namespace _project.Scripts.Presentation.View
{
    public class UnityHealth : MonoBehaviour , IHealth
    {
        public bool IsAlive { get; private set;}
        public float MaxHealth { get; private set;}
        public float CurrentHealth { get; set;}
        public void Heal(float amount)
        {
            CurrentHealth +=  amount;
        }
    }
}