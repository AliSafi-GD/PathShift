namespace _project.Scripts.Domain.Interfaces
{
    public interface IAttacker:IBehavior
    {
        void Attack(IAttackable target);
    }
}