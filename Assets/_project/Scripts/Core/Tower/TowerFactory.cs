using UnityEngine;

namespace _project.Scripts.Core.Tower
{
    public class TowerFactory : MonoBehaviour
    {
        [SerializeField] private TowerView viewPrefab;
        [SerializeField] private TowerConfig defaultConfig;
        [SerializeField] private ProjectileFactory projectileFactory;

        public TowerConfig DefaultConfig => defaultConfig;

        public (Tower tower, TowerView view) Create(Vector3 worldPosition, TowerConfig config = null)
        {
            var cfg = config != null ? config : defaultConfig;
            var view = SpawnView(worldPosition, cfg);
            var tower = BuildTower(worldPosition, cfg);
            return (tower, view);
        }

        // فقط view رو اسپان می‌کنه. برای آپگرید (که می‌خوایم view عوض بشه) استفاده میشه.
        public TowerView SpawnView(Vector3 worldPosition, TowerConfig config)
        {
            var prefab = (config != null && config.viewPrefab != null) ? config.viewPrefab : viewPrefab;
            var view = Instantiate(prefab);
            view.transform.position = worldPosition;
            return view;
        }

        // فقط دیتای Tower رو می‌سازه (بدون اسپان view). برای آپگرید استفاده میشه.
        public Tower BuildTower(Vector3 worldPosition, TowerConfig config)
        {
            var cfg = config != null ? config : defaultConfig;
            var policy = new ClosestTargetPolicy();
            var weapon = new CannonWeapon(projectileFactory, cfg.damage);
            return new Tower(
                position: worldPosition,
                fireRate: cfg.fireRate,
                range: cfg.range,
                targetingPolicy: policy,
                weapon: weapon);
        }
    }
}
