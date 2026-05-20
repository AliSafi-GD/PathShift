using _project.Scripts.Core.GridSystem.Roles;

namespace _project.Scripts.Core.GridSystem
{
    /// <summary>
    /// One logical cell of the grid. Knows its position, its design-time role,
    /// and what (if anything) currently occupies it. Knows nothing about
    /// world space, pathfinding, or mover types.
    /// </summary>
    public sealed class GridCell
    {
        public GridPosition Position { get; }
        public CellRole Role { get; }
        public IOccupant Occupant { get; private set; }

        public bool IsEmpty => Occupant == null;

        public GridCell(GridPosition position, CellRole role)
        {
            Position = position;
            Role = role;
        }

        /// <summary>
        /// Attempts to place an occupant on this cell.
        /// Fails if the cell is already occupied or its role rejects the occupant.
        /// </summary>
        public bool TryPlace(IOccupant occupant)
        {
            if (occupant == null) return false;
            if (!IsEmpty) return false;
            if (!Role.AllowsOccupant(occupant)) return false;

            Occupant = occupant;
            return true;
        }

        public void Clear() => Occupant = null;
    }
}
