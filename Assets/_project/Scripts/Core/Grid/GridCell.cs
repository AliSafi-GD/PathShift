namespace _project.Scripts.Domain.Grid
{
    public class GridCell
    {
        public int Id {get; private set;}
        public GridPosition Position { get; }
        public bool IsWalkable { get; private set; }

        public GridCell(int id,GridPosition position, bool isWalkable = true)
        {
            Id = id;
            Position = position;
            IsWalkable = isWalkable;
        }

        public void Block()
        {
            IsWalkable = false;
        }

        public void Unblock()
        {
            IsWalkable = true;
        }
    }
}