using _project.Scripts.Presentation.View;
using Unity.VisualScripting;
using UnityEngine;

namespace _project.Scripts.Core.Enemy
{
    public class EnemyFactory : MonoBehaviour
    {
        [SerializeField] private EnemyView prefab;
        
        private Transform startSpawnTransform;

        public void Setup(Transform startSpawnTransform)
        {
            this.startSpawnTransform = startSpawnTransform;
        }
        public Domain.Entitties.Enemy CreateEnemy()
        {
            EnemyView instance = Instantiate(prefab);
            var unityMovement = instance.GetOrAddComponent<UnityMovement>();
            var unityHealth = instance.GetOrAddComponent<UnityHealth>();
            var unityAttacker = instance.GetOrAddComponent<UnityAttacker>();
            var unityAttackable = instance.GetOrAddComponent<UnityAttackable>();
            var enemy = new Domain.Entitties.Enemy(unityMovement, unityHealth, unityAttackable,unityAttacker,instance);
            return enemy;
        }
    }
}