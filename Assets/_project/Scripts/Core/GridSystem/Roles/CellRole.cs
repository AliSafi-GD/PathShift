namespace _project.Scripts.Core.GridSystem.Roles
{
    /// <summary>
    /// The design-time purpose of a cell. Decides what kind of occupants it accepts.
    /// Subclass to add new roles without touching existing code.
    /// </summary>
    public abstract class CellRole
    {
        public virtual bool AllowsOccupant(IOccupant occupant) => true;
    }
}
