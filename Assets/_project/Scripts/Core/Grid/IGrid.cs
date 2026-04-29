using System.Collections.Generic;

namespace _project.Scripts.Domain.Grid
{
    public interface IGrid
    {
        int Width { get; }
        int Height { get; }

        GridCell GetCell(GridPosition position);
        bool IsInside(GridPosition position);
        List<GridCell> GetAllCells();
    }
}