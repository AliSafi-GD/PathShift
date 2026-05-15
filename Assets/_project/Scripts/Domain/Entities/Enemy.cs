using _project.Scripts.Domain.Interfaces;

namespace _project.Scripts.Domain.Entities
{
    public interface IEnemyView { }

    public class Enemy
    {
        public IMovable Movement { get; }
        public IHealth Health { get; }
        public IAttackable Attackable { get; }
        public IAttacker Attacker { get; }
        public IEnemyView View { get; }

        public Enemy(IMovable movement, IHealth health, IAttackable attackable, IAttacker attacker, IEnemyView view)
        {
            Movement = movement;
            Health = health;
            Attackable = attackable;
            Attacker = attacker;
            View = view;
        }
    }
}
