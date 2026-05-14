using _project.Scripts.Presentation.View;
using Unity.VisualScripting;
using UnityEngine;

namespace _project.Scripts.Core.Tower
{
    public class TowerFactory : MonoBehaviour
    {
        [SerializeField] private TowerView viewPrefab;
        [SerializeField] private TowerConfig defaultConfig;
        [SerializeField] private ProjectileFactory projectileFactory;
        [SerializeField] private MortarProjectileFactory mortarProjectileFactory;

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

            // drop-from-sky animation: قبل از render اول، position رو بالا ببر و tween کن.
            var anim = view.gameObject.GetOrAddComponent<TowerSpawnAnimator>();
            anim.Prepare();
            anim.Play();

            return view;
        }

        // فقط دیتای Tower رو می‌سازه (بدون اسپان view). برای آپگرید استفاده میشه.
        public Tower BuildTower(Vector3 worldPosition, TowerConfig config)
        {
            var cfg = config != null ? config : defaultConfig;
            var policy = new ClosestTargetPolicy();
            IWeapon weapon = cfg.weaponKind == WeaponKind.Mortar
                ? (IWeapon)new MortarWeapon(mortarProjectileFactory, cfg.damage,
                                            cfg.splashRadius, cfg.arcHeight, cfg.mortarTravelTime)
                : new CannonWeapon(projectileFactory, cfg.damage);

            return new Tower(
                position: worldPosition,
                fireRate: cfg.fireRate,
                range: cfg.range,
                targetingPolicy: policy,
                weapon: weapon);
        }
    }
}
