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
                var clamped = Mathf.Clamp(value, 0f, maxHealth);
                if (Mathf.Approximately(clamped, currentHealth)) return;
                currentHealth = clamped;
                OnHealthChanged?.Invoke(currentHealth, maxHealth);
                if (!died && currentHealth <= 0f)
                {
                    died = true;
                    OnDied?.Invoke();
                }
            }
        }

        public event Action OnDied;
        public event Action<float, float> OnHealthChanged;

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        // ست کردن قبل از Awake: مقدار اولیه health هم باهاش پر میشه.
        // ست کردن بعد از Awake: فقط سقف عوض میشه (currentHealth دست‌نخورده).
        public void SetMaxHealth(float value, bool resetCurrent = true)
        {
            maxHealth = Mathf.Max(1f, value);
            if (resetCurrent) currentHealth = maxHealth;
            else currentHealth = Mathf.Min(currentHealth, maxHealth);
            died = false;
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