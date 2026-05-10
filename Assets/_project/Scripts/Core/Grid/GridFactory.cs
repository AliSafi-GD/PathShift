using System.Collections.Generic;
using UnityEngine;

namespace _project.Scripts.Domain.Grid
{
    public class CellViewRegistry
    {
        private readonly Dictionary<int, CellView> cellViews = new();

        public void Register(Dictionary<int, CellView> views)
        {
            cellViews.Clear();
            foreach (var kv in views)
                cellViews[kv.Key] = kv.Value;
        }

        public bool TryGet(int cellId, out CellView cellView)
        {
            return cellViews.TryGetValue(cellId, out cellView);
        }
    }
    public class GridFactory : MonoBehaviour
    {
        [SerializeField] private CellView prefab;
        [SerializeField] private Transform parent;

        public Dictionary<int,CellView> CreateVisual(List<GridCell> cells)
        {
            Dictionary<int, CellView> cellViews = new();
            foreach (var gridCell in cells)
            {
                var position = new Vector3(gridCell.Position.X,0,gridCell.Position.Y);
                var cellView = Instantiate(prefab, position, Quaternion.identity, parent);
                cellView.Init(gridCell);
                cellView.name = $"Cell id[{gridCell.Id}] POS[{gridCell.Position.X},{gridCell.Position.Y}]";
                cellViews[gridCell.Id] = cellView;
            }
            return cellViews;
        }
    }
}