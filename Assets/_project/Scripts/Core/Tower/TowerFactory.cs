using _project.Scripts.Presentation.View;
using Unity.VisualScripting;
using UnityEngine;

namespace _project.Scripts.Core.Tower
{
    public interface ITowerFactory
    {
        TowerConfig DefaultConfig { get; }
        (Tower tower, TowerView view) Create(Vector3 worldPosition, TowerConfig config = null);
        TowerView SpawnView(Vector3 worldPosition, TowerConfig config);
        Tower BuildTower(Vector3 worldPosition, TowerConfig config);
    }

    public class TowerFactory : MonoBehaviour, ITowerFactory
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

        // Spawns only the view (used by upgrade, which swaps the view).
        public TowerView SpawnView(Vector3 worldPosition, TowerConfig config)
        {
            var prefab = (config != null && config.viewPrefab != null) ? config.viewPrefab : viewPrefab;
            var view = Instantiate(prefab);
            view.transform.position = worldPosition;

            // Drop-from-sky animation: prepare the transform before the first render.
            var anim = view.gameObject.GetOrAddComponent<TowerSpawnAnimator>();
            anim.Prepare();
            anim.Play();

            return view;
        }

        // Builds only the Tower domain model (no view), used by upgrade.
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
