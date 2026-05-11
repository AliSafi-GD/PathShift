using _project.Scripts.Core.Map;
using _project.Scripts.Core.Tower;
using _project.Scripts.Presentation.View;
using UnityEngine;

namespace _project.Scripts.Domain.Map
{
    public class MapView : MonoBehaviour , IMapView
    {
        [SerializeField] private MainTowerView mainTowerViewInstance;
        public IMainTowerView GetMainTowerView()
        {
            return mainTowerViewInstance;
        }
    }
}