using System;
using _project.Scripts.Domain.Interfaces;

namespace _project.Scripts.Domain.Combat
{
    // Pure C# health model. No Unity dependency — usable from tests and from
    // adapter MonoBehaviours alike. UnityHealth is the scene-side wrapper.
    public class Health : IHealth
    {
        private float max;
        private float current;
        private bool died;

        public bool IsAlive => current > 0f;
        public float MaxHealth => max;
        public float CurrentHealth => current;

        public event Action OnDied;
        public event Action<float, float> OnHealthChanged;

        public Health(float maxHealth)
        {
            max = Math.Max(1f, maxHealth);
            current = max;
        }

        public void TakeDamage(float amount)
        {
            if (amount <= 0f) return;
            SetCurrent(current - amount);
        }

        public void Heal(float amount)
        {
            if (amount <= 0f) return;
            if (!IsAlive) return;
            SetCurrent(current + amount);
        }

        // Resize the cap. resetCurrent=true also refills to full and clears the
        // died flag — used at spawn time when reusing a pooled instance.
        public void SetMax(float value, bool resetCurrent = true)
        {
            max = Math.Max(1f, value);
            if (resetCurrent)
            {
                current = max;
                died = false;
            }
            else
            {
                current = Math.Min(current, max);
            }
        }

        private void SetCurrent(float value)
        {
            var clamped = Clamp(value, 0f, max);
            if (Math.Abs(clamped - current) < float.Epsilon) return;
            current = clamped;
            OnHealthChanged?.Invoke(current, max);
            if (!died && current <= 0f)
            {
                died = true;
                OnDied?.Invoke();
            }
        }

        private static float Clamp(float value, float min, float max)
            => value < min ? min : (value > max ? max : value);
    }
}
