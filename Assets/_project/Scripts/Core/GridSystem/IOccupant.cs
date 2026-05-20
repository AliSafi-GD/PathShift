namespace _project.Scripts.Core.GridSystem
{
    /// <summary>
    /// Anything that can sit on a <see cref="GridCell"/> — towers, rocks, decorations, traps...
    /// An occupant describes its own physical blocking properties; pathfinders consult these
    /// based on the kind of mover (ground/air) they represent.
    /// </summary>
    public interface IOccupant
    {
        bool BlocksGround { get; }
        bool BlocksAir { get; }
    }
}
