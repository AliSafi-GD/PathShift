using System.Collections.Generic;

namespace _project.Scripts.Domain.Grid
{
    public class GridService : IGrid
    {
        private readonly List<GridCell> cells;
        private readonly Dictionary<GridPosition, GridCell> byPosition;

        public GridService(List<GridCell> cells)
        {
            this.cells = cells;
            byPosition = new Dictionary<GridPosition, GridCell>(cells.Count);
            foreach (var c in cells)
                byPosition[c.Position] = c;
        }

        public GridCell GetCell(GridPosition position)
        {
            return byPosition.TryGetValue(position, out var c) ? c : null;
        }

        public bool IsInside(GridPosition position)
        {
            return byPosition.ContainsKey(position);
        }

        public List<GridCell> GetAllCells()
        {
            return new List<GridCell>(cells);
        }

        public List<GridCell> GetWalkableCells()
        {
            var result = new List<GridCell>(cells.Count);
            foreach (var c in cells)
                if (c.GridCellType != GridCellType.Block)
                    result.Add(c);
            return result;
        }

        public void SetWalkable(GridCell gridCell, bool isWalkable)
        {
            if (gridCell == null) return;
            if (!IsInside(gridCell.Position)) return;

            if (isWalkable) gridCell.Unblock();
            else gridCell.Block();
        }
    }
}