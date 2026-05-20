using System.Collections.Generic;
using _project.Scripts.Core.GridSystem.Roles;

namespace _project.Scripts.Core.GridSystem
{
    /// <summary>
    /// Read/write access to the grid as a collection of cells.
    /// Knows nothing about neighbors, pathfinding, or world space —
    /// those are concerns of the consumers (pathfinders, views, ...).
    /// </summary>
    public interface IGrid
    {
        GridCell GetCell(GridPosition position);
        bool IsInside(GridPosition position);

        IReadOnlyCollection<GridCell> AllCells { get; }
        IEnumerable<GridCell> CellsByRole<TRole>() where TRole : CellRole;
    }
}
