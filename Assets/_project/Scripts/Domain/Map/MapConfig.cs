using UnityEngine;

namespace _project.Scripts.Domain.Map
{
    [CreateAssetMenu(fileName = "MapConfig", menuName = "Map/Map Config")]
    public class MapConfig : ScriptableObject
    {
        [SerializeField] private string mapName;
        [SerializeField] private Sprite previewIcon;
        [SerializeField] private MapView mapPrefab;
        [SerializeField] private GridData gridData;

        public string MapName => mapName;
        public Sprite PreviewIcon => previewIcon;
        public MapView MapPrefab => mapPrefab;
        public GridData GridData => gridData;
    }
}