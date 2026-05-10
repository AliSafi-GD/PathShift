using System.Collections.Generic;

namespace _project.Scripts.Domain.Grid
{
    public class GridService : IGrid
    {
        //private readonly GridCell[,] _cells;

        // public int Width { get; }
        // public int Height { get; }

        List<GridCell> cells = new List<GridCell>();
        public GridService(List<GridCell> cells)
        {
            this.cells = cells;
            // InitializeCells();
        }

        // private void InitializeCells()
        // {
        //     int id = 1;
        //     for (int x = 0; x < Width; x++)
        //     {
        //         for (int y = 0; y < Height; y++)
        //         {
        //             var position = new GridPosition(x, y);
        //             _cells[x, y] = new GridCell(id,position, GridCellType.Walkable);
        //             id++;
        //         }
        //     }
        // }

        public GridCell GetCell(GridPosition position)
        {
            return null;
        }

        public bool IsInside(GridPosition position)
        {
            return false;
        }

        public List<GridCell> GetAllCells()
        {
            List<GridCell> clonedCells = new List<GridCell>(cells);
            return cells;
        }
        public List<GridCell> GetWalkableCells()
        {
            List<GridCell> cellsClone = new List<GridCell>();

            foreach (var gridCell in cells)
            {
                if (gridCell.gridCellType != GridCellType.Block)
                    cellsClone.Add(gridCell);
            }

            return cellsClone;
        }
        public void SetWalkable(GridCell gridCell,bool isWalkable)
        {
            if (gridCell == null)
                return;

            var pos = gridCell.Position;

            if (!IsInside(pos))
                return;

            if (gridCell.gridCellType != GridCellType.Walkable)
                cells[cells.IndexOf(gridCell)].Unblock();
            else
                cells[cells.IndexOf(gridCell)].Block();
        }
    }
}