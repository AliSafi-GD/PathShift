using UnityEngine;

namespace _project.Scripts.Domain.Map
{
    public class HardcodedMapProvider : MonoBehaviour, IMapProvider
    {
        [SerializeField] private MapConfig selectedMap;

        public MapConfig GetSelectedMap() => selectedMap;
    }
}