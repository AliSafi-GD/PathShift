using System;
using System.Collections.Generic;
using _project.Scripts.Core.Pathfinding.Main;
using UnityEngine;

namespace _project.Scripts.Core.Context
{
    public class ServiceLocator
    {
        private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

        public void Register<TService>(TService instance)
        {
            _services[typeof(TService)] = instance;
            Debug.Log($"[GameContext] {typeof(TService).Name} registered");
        }

        public TService Resolve<TService>()
        {
            if (_services.TryGetValue(typeof(TService), out var instance))
            {
                return (TService)instance;
            }

            throw new Exception($"[GameContext] Service {typeof(TService).Name} not registered");
        }
    }
}