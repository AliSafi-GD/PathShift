namespace _project.Scripts.Domain.Interfaces
{
    public interface IHealth : IBehavior
    {
        bool IsAlive { get; }
        float MaxHealth { get; }
        float CurrentHealth { get; }
        void Heal(float amount);
    }
}