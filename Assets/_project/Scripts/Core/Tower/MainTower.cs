using System;
using _project.Scripts.Domain.Interfaces;
using UnityEngine;

namespace _project.Scripts.Core.Tower
{
    public interface IMainTowerView { }

    public class MainTower
    {
        public IHealth Health { get; }
        public IAttackable Attackable { get; }
        public IMainTowerView View { get; }

        public event Action OnDestroyed;

        public MainTower(IHealth health, IAttackable attackable, IMainTowerView view)
        {
            Health = health;
            Attackable = attackable;
            View = view;

            Health.OnDied += HandleDied;
        }

        private void HandleDied()
        {
            Debug.Log("game over");
            // OnDestroyed?.Invoke();
        }
    }
}
