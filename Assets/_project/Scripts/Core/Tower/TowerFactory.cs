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

            var view = Instantiate(viewPrefab);
            view.transform.position = worldPosition;

            var policy = new ClosestTargetPolicy();
            var weapon = new CannonWeapon(projectileFactory, cfg.damage);

            var tower = new Tower(
                position: worldPosition,
                fireRate: cfg.fireRate,
                range: cfg.range,
                targetingPolicy: policy,
                weapon: weapon);

            return (tower, view);
        }
    }
}
