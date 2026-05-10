using System;
using System.Collections.Generic;
using _project.Scripts.Domain.Interfaces;
using _project.Scripts.Presentation.View;
using Unity.VisualScripting;
using UnityEngine;

namespace _project.Scripts.Core.Tower
{
    
    public class MainTower : MonoBehaviour
    {
        private readonly List<IBehavior> behaviours;

        private void Start()
        {
            behaviours.Add(transform.GetOrAddComponent<UnityHealth>());
            behaviours.Add(transform.GetOrAddComponent<UnityAttackable>());
        }
    }
}