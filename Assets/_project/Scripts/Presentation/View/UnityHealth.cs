using System;
using _project.Scripts.Domain.Interfaces;
using UnityEngine;

namespace _project.Scripts.Presentation.View
{
    public class UnityHealth : MonoBehaviour, IHealth
    {
        [SerializeField] private float maxHealth = 100f;

        private float currentHealth;
        private bool died;

        public bool IsAlive => CurrentHealth > 0f;
        public float MaxHealth => maxHealth;

        public float CurrentHealth
        {
            get => currentHealth;
            set
            {
                currentHealth = Mathf.Clamp(value, 0f, maxHealth);
                if (!died && currentHealth <= 0f)
                {
                    died = true;
                    OnDied?.Invoke();
                }
            }
        }

        public event Action OnDied;

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        public void TakeDamage(float amount)
        {
            if (amount <= 0f) return;
            CurrentHealth -= amount;
        }

        public void Heal(float amount)
        {
            if (amount <= 0f) return;
            if (!IsAlive) return; // can't heal a corpse
            CurrentHealth += amount;
        }
    }
}