using System;
using _project.Scripts.Domain.Combat;
using _project.Scripts.Domain.Interfaces;
using UnityEngine;

namespace _project.Scripts.Presentation.View
{
    // Scene-side adapter: exposes a serialized MaxHealth slot and forwards every
    // call to a plain-C# Health instance. The real model lives in Domain/Combat.
    public class UnityHealth : MonoBehaviour, IHealth
    {
        [SerializeField] private float maxHealth = 100f;

        private Health health;
        private Health Inner => health ??= new Health(maxHealth);

        public bool IsAlive => Inner.IsAlive;
        public float MaxHealth => Inner.MaxHealth;
        public float CurrentHealth => Inner.CurrentHealth;

        public event Action OnDied
        {
            add => Inner.OnDied += value;
            remove => Inner.OnDied -= value;
        }

        public event Action<float, float> OnHealthChanged
        {
            add => Inner.OnHealthChanged += value;
            remove => Inner.OnHealthChanged -= value;
        }

        public void TakeDamage(float amount) => Inner.TakeDamage(amount);
        public void Heal(float amount) => Inner.Heal(amount);

        public void SetMaxHealth(float value, bool resetCurrent = true)
        {
            maxHealth = Mathf.Max(1f, value);
            Inner.SetMax(maxHealth, resetCurrent);
        }
    }
}
