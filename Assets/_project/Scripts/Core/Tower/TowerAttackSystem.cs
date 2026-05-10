using System.Collections.Generic;
using _project.Scripts.Core.Enemy;
using UnityEngine;

namespace _project.Scripts.Core.Tower
{
    public class TowerAttackSystem : MonoBehaviour
    {
        public List<Tower> towers =  new List<Tower>();
        // public EnemyService enemyService;

        private void Update()
        {
            float dt = Time.deltaTime;
            // var enemies = enemyService.GetAliveEnemies();

            foreach (var tower in towers)
            {
                // tower.Tick(dt, enemies);
            }
        }
    }

}