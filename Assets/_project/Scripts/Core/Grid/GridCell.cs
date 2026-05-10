namespace _project.Scripts.Domain.Grid
{
    public enum GridCellType
    {
        StartPoint,
        EndPoint,
        Walkable,
        Block
    }
    public class GridCell
    {
        public int Id {get; private set;}
        public GridPosition Position { get; }
        public GridCellType gridCellType { get; private set; }

        public GridCell(int id,GridPosition position,GridCellType gridCellType)
        {
            Id = id;
            Position = position;
            this.gridCellType = gridCellType;
        }

        public void Block()
        {
            gridCellType = GridCellType.Block;
        }

        public void Unblock()
        {
            gridCellType = GridCellType.Walkable;
        }
    }
}