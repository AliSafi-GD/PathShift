using UnityEngine;

namespace _project.Scripts.Core.Enemy
{
    [CreateAssetMenu(fileName = "EnemySpawnConfig", menuName = "Configs/Enemy Spawn Config")]
    public class EnemySpawnConfig : ScriptableObject
    {
        public GameObject enemyPrefab;
        public int enemiesPerWave = 5;
        public float spawnInterval = 1.0f;
        public float waveInterval = 3.0f;
    }
}