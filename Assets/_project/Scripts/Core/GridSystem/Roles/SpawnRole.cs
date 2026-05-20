namespace _project.Scripts.Core.GridSystem.Roles
{
    /// <summary>
    /// Enemy spawn point. Nothing can be placed here.
    /// </summary>
    public sealed class SpawnRole : CellRole
    {
        public override bool AllowsOccupant(IOccupant occupant) => false;
    }
}
