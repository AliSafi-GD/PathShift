using _project.Scripts.Core.Map;
using UnityEngine;

namespace _project.Scripts.Domain.Map
{
    public class MapFactory : MonoBehaviour , IMapFactory
    {
        public IMapView CreateMap(MapConfig mapConfig)
        {
            var spawnedMap = Instantiate(mapConfig.MapPrefab);
            spawnedMap.name = $"Map_{mapConfig.MapName}";
            return spawnedMap;
        }
    }
}