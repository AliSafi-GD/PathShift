using System.Collections.Generic;
using UnityEngine;

namespace _project.Scripts.Domain.Grid
{
    public class CellViewRegistry
    {
        public Dictionary<int,CellView> CellViews = new Dictionary<int, CellView>();
    }
    public class GridFactory
    {
        private CellView prefab;
        private Transform parent;

        public GridFactory(CellView prefab, Transform parent)
        {
            this.prefab = prefab;
            this.parent = parent;
        }

        public Dictionary<int,CellView> CreateVisual(List<GridCell> cells)
        {
            Dictionary<int, CellView> cellViews = new();
            foreach (var gridCell in cells)
            {
                var position = new Vector3(gridCell.Position.X,0,gridCell.Position.Y);
                var cellView = GameObject.Instantiate(prefab, position, Quaternion.identity, parent);
                cellViews[gridCell.Id] = cellView;
            }
            return cellViews;
        }
    }
}