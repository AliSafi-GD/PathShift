using _project.Scripts.Core.Economy;
using _project.Scripts.Presentation.View;
using UnityEngine;

namespace _project.Scripts.Core.Enemy
{
    [CreateAssetMenu(fileName = "EnemyConfig", menuName = "Configs/Enemy Config")]
    public class EnemyConfig : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string displayName = "Enemy";
        [SerializeField] private Sprite icon;
        [SerializeField] private EnemyView prefab;

        [Header("Stats")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private float damage = 10f;
        [SerializeField] private float attackInterval = 1f;

        [Header("Reward")]
        [Tooltip("پاداش وقتی این enemy توسط tower کشته میشه (نه با رسیدن به base).")]
        [SerializeField] private Cost killReward = new Cost(CurrencyType.Coin, 10);

        public string DisplayName => displayName;
        public Sprite Icon => icon;
        public EnemyView Prefab => prefab;
        public float MaxHealth => maxHealth;
        public float MoveSpeed => moveSpeed;
        public float Damage => damage;
        public float AttackInterval => attackInterval;
        public Cost KillReward => killReward;
    }
}
