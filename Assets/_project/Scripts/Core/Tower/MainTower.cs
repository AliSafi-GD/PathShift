using System;
using System.Collections.Generic;
using _project.Scripts.Domain.Interfaces;
using UnityEngine;

namespace _project.Scripts.Core.Tower
{
    public interface IMainTowerView { }

    public class MainTower
    {
        private readonly IHealth health;
        private readonly IAttackable attackable;
        private readonly IMainTowerView view;
        private readonly List<IBehavior> behaviours;

        public event Action OnDestroyed;

        public IHealth Health => health;

        public MainTower(IHealth health, IAttackable attackable, IMainTowerView view)
        {
            this.health = health;
            this.attackable = attackable;
            this.view = view;

            behaviours = new List<IBehavior>
            {
                health,
                attackable
            };

            this.health.OnDied += HandleDied;
        }

        private void HandleDied()
        {
            Debug.Log("game over");
            // OnDestroyed?.Invoke();
        }

        public T GetBehavior<T>() where T : IBehavior
        {
            var behavior = behaviours.Find(b => b is T);
            return behavior is T t ? t : default;
        }

        public IMainTowerView GetView() => view;
    }
}