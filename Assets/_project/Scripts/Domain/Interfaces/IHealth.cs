using System;

namespace _project.Scripts.Domain.Interfaces
{
    public interface IHealth : IBehavior
    {
        bool IsAlive { get; }
        float MaxHealth { get; }
        float CurrentHealth { get; }
        event Action OnDied;
        // (current, max)
        event Action<float, float> OnHealthChanged;
        void TakeDamage(float amount);
        void Heal(float amount);
    }
}