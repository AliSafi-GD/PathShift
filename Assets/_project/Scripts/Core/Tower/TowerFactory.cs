using UnityEngine;

namespace _project.Scripts.Core.Tower
{
    public class TowerFactory : MonoBehaviour
    {
        [SerializeField] private TowerView view;

        public TowerView CreateTower(Transform place)
        {
            var towerView = Instantiate(view, place);
            return towerView;
        }
    }
}