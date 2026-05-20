using System.Collections.Generic;
using _project.Scripts.Core.GridSystem.Roles;

namespace _project.Scripts.Core.GridSystem
{
    /// <summary>
    /// Default <see cref="IGrid"/> implementation backed by a dictionary for O(1) lookup.
    /// </summary>
    public sealed class GridManager : IGrid
    {
        private readonly Dictionary<GridPosition, GridCell> cellsByPosition;
        private readonly List<GridCell> allCells;

        public GridManager(IEnumerable<GridCell> cells)
        {
            allCells = new List<GridCell>(cells);
            cellsByPosition = new Dictionary<GridPosition, GridCell>(allCells.Count);
            foreach (var cell in allCells)
                cellsByPosition[cell.Position] = cell;
        }

        public IReadOnlyCollection<GridCell> AllCells => allCells;

        public GridCell GetCell(GridPosition position)
            => cellsByPosition.TryGetValue(position, out var cell) ? cell : null;

        public bool IsInside(GridPosition position)
            => cellsByPosition.ContainsKey(position);

        public IEnumerable<GridCell> CellsByRole<TRole>() where TRole : CellRole
        {
            foreach (var cell in allCells)
                if (cell.Role is TRole)
                    yield return cell;
        }
    }
}
