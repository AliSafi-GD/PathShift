namespace _project.Scripts.Core.GridSystem.Roles
{
    /// <summary>
    /// The main core / base the player defends. Nothing can be placed here.
    /// </summary>
    public sealed class CoreRole : CellRole
    {
        public override bool AllowsOccupant(IOccupant occupant) => false;
    }
}
