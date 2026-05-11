namespace _project.Scripts.Domain.Interfaces
{
    public interface IAttackable : IBehavior
    {
        void ReceiveDamage(float damage);
    }
}
