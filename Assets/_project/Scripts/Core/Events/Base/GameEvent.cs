using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace _project.Scripts.Core.Events.Base
{
    public interface IGameEventListener
    {
    }

    public interface IGameEventListener<in T> : IGameEventListener
    {
        void OnEventRaised(T value);
    }

    public interface IEventBus
    {
        void Raise<T>(T data);
        void RegisterListener<T>(IGameEventListener<T> listener);
        void UnregisterListener<T>(IGameEventListener listener);
        void RegisterListenerMultiple(object listener);
        void UnregisterListenerMultiple(object listener);
    }

    public class GameEventBus : IEventBus
    {
        private readonly Dictionary<Type, List<IGameEventListener>> Listeners =
            new Dictionary<Type, List<IGameEventListener>>();

        public void Raise<T>(T data)
        {
            if (!Listeners.ContainsKey(typeof(T)))
            {
                Debug.LogError($"listener {typeof(T)} not found");
            }

            var gameEventListeners = Listeners[typeof(T)];
            gameEventListeners.ForEach(listener => { ((IGameEventListener<T>)listener).OnEventRaised(data); });
        }

        public void RegisterListener<T>(IGameEventListener<T> listener)
        {
            if (!Listeners.ContainsKey(typeof(T)))
            {
                Listeners[typeof(T)] = new List<IGameEventListener>();
            }

            Listeners[typeof(T)].Add(listener);
        }

        public void UnregisterListener<T>(IGameEventListener listener)
        {
            if (!Listeners.ContainsKey(typeof(T)))
            {
                Debug.LogError($"{typeof(T)} : key not found");
                return;
            }

            if (Listeners[typeof(T)].Contains(listener))
            {
                Listeners[typeof(T)].Remove(listener);
                return;
            }

            Debug.LogError($"{listener.GetType()} : doesn't exist");
        }


        public void RegisterListenerMultiple(object listener)
        {
            var listenerType = listener.GetType();
            var interfaces = listenerType.GetInterfaces();

            foreach (var interfaceType in interfaces)
            {
                if (interfaceType.IsGenericType &&
                    interfaceType.GetGenericTypeDefinition() == typeof(IGameEventListener<>))
                {
                    var eventType = interfaceType.GetGenericArguments()[0];

                    var registerMethod = typeof(GameEventBus)
                        .GetMethod(nameof(RegisterListener), BindingFlags.Public | BindingFlags.Static)
                        ?.MakeGenericMethod(eventType);

                    registerMethod?.Invoke(null, new[] { listener });
                }
            }
        }

        public void UnregisterListenerMultiple(object listener)
        {
            var listenerType = listener.GetType();
            var interfaces = listenerType.GetInterfaces();

            foreach (var interfaceType in interfaces)
            {
                if (interfaceType.IsGenericType &&
                    interfaceType.GetGenericTypeDefinition() == typeof(IGameEventListener<>))
                {
                    var eventType = interfaceType.GetGenericArguments()[0];

                    var unregisterMethod = typeof(GameEventBus)
                        .GetMethod(nameof(UnregisterListener), BindingFlags.Public | BindingFlags.Static)
                        ?.MakeGenericMethod(eventType);

                    unregisterMethod?.Invoke(null, new[] { listener });
                }
            }
        }
    }
}