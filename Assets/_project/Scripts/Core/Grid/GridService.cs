using System.Collections.Generic;

namespace _project.Scripts.Domain.Grid
{
    public class GridService : IGrid
    {
        private readonly GridCell[,] _cells;

        public int Width { get; }
        public int Height { get; }

        public GridService(int width, int height)
        {
            Width = width;
            Height = height;
            _cells = new GridCell[width, height];

            InitializeCells();
        }

        private void InitializeCells()
        {
            int id = 1;
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    var position = new GridPosition(x, y);
                    _cells[x, y] = new GridCell(id,position, true);
                    id++;
                }
            }
        }

        public GridCell GetCell(GridPosition position)
        {
            if (!IsInside(position))
                return null;

            return _cells[position.X, position.Y];
        }

        public bool IsInside(GridPosition position)
        {
            return position.X >= 0 && position.X < Width &&
                   position.Y >= 0 && position.Y < Height;
        }

        public List<GridCell> GetAllCells()
        {
            List<GridCell> cells = new List<GridCell>();
            foreach (var cell in _cells)
                cells.Add(cell);
            
            return cells;
        }
    }
}