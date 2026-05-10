namespace _project.Scripts.Gameplay.Map
{
    using UnityEngine;
    using System.Collections.Generic;

    [CreateAssetMenu(menuName = "Game/Map")]
    public class MapData : ScriptableObject
    {
        public int width;
        public int height;

        public List<CellData> cells = new();
    }

    [System.Serializable]
    public class CellData
    {
        public int x;
        public int y;
        public CellType type;
    }
    public enum CellType
    {
        Empty,
        Start,
        End,
        Blocked
    }

}