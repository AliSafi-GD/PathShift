using System.Collections.Generic;
using _project.Scripts.Core.Enemy;
using _project.Scripts.Domain.Interfaces;
using UnityEngine;

namespace _project.Scripts.Domain.Entitties
{
    public interface IEnemyView { }
    public class Enemy
    {
        private readonly IMovable movable;
        private IHealth health;
        private IAttackable attackable;
        private IAttacker attacker;
        private readonly IEnemyView enemyView;
        private readonly List<IBehavior> behaviours;
        public Enemy(IMovable movable, IHealth health, IAttackable attackable, IAttacker attacker, IEnemyView enemyView)
        {
            this.movable = movable;
            this.attackable = attackable;
            this.attacker = attacker;
            this.enemyView = enemyView;
            this.health = health;
            behaviours = new List<IBehavior>();
            behaviours.Add(movable);
            behaviours.Add(health);
            behaviours.Add(attacker);
            behaviours.Add(attackable);
        }

        public T GetBehavior<T>() where T : IBehavior
        {
            var behavior = behaviours.Find(b => b is T);
            return behavior is T t ? t : default;
        }

        public IEnemyView GetEnemyView() => enemyView;
    }
}