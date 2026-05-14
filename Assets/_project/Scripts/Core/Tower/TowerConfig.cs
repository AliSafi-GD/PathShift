using UnityEngine;

namespace _project.Scripts.Core.Tower
{
    [CreateAssetMenu(fileName = "TowerConfig", menuName = "Configs/Tower Config")]
    public class TowerConfig : ScriptableObject
    {
        [Header("Combat")]
        public float fireRate = 1f;
        public float damage = 10f;
        public float range = 5f;

        [Header("Projectile")]
        public float projectileSpeed = 12f;
        public float hitRadius = 0.2f;

        [Header("Visuals (optional — TowerFactory uses its default if null)")]
        [Tooltip("پریفب view در این لول. اگه null باشه TowerFactory.defaultViewPrefab استفاده میشه.")]
        public TowerView viewPrefab;
        [Tooltip("پریفب پیش‌نمایش (موقع hover). اگه null باشه از previewPrefab روی کارت استفاده میشه.")]
        public GameObject previewPrefab;
    }
}
