using UnityEngine;

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
        public int Id { get; }
        public GridPosition Position { get; }       // برای A* و logic داخلی
        public Vector3 WorldPosition { get; }       // ⭐ اضافه میشه - برای view و حرکت
        public GridCellType GridCellType { get; private set; }
    
        public GridCell(int id, GridPosition pos, Vector3 worldPos, GridCellType type)
        {
            Id = id;
            Position = pos;
            WorldPosition = worldPos;
            GridCellType = type;
        }

        public void Block()
        {
            GridCellType = GridCellType.Block;
        }

        public void Unblock()
        {
            GridCellType = GridCellType.Walkable;
        }
    }
}