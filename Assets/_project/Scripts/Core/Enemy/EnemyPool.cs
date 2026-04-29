using System.Collections.Generic;
using UnityEngine;

namespace _project.Scripts.Core.Enemy
{
    public class EnemyPool : MonoBehaviour
    {
        [SerializeField] private EnemyView prefab;
        [SerializeField] private int initialSize = 10;

        private readonly Queue<EnemyView> _pool = new();

        private void Awake()
        {
            for (int i = 0; i < initialSize; i++)
                CreateInstance();
        }

        public EnemyView Get(Vector3 position)
        {
            EnemyView enemy = _pool.Count > 0 ? _pool.Dequeue() : CreateInstance();
            enemy.transform.position = position;
            enemy.gameObject.SetActive(true);
            return enemy;
        }

        public void Return(EnemyView enemy)
        {
            enemy.gameObject.SetActive(false);
            enemy.transform.SetParent(transform);
            _pool.Enqueue(enemy);
        }

        private EnemyView CreateInstance()
        {
            EnemyView instance = Instantiate(prefab, transform);
            instance.gameObject.SetActive(false);
            return instance;
        }
    }
}
