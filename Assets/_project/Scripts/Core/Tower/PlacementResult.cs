using _project.Scripts.Domain.Grid;

namespace _project.Scripts.Core.Tower
{
    public readonly struct PlacementResult
    {
        public readonly bool Success;
        public readonly PlacementFailure Failure;
        public readonly GridCell PlacedOn;

        private PlacementResult(bool success, PlacementFailure failure, GridCell placedOn)
        {
            Success = success;
            Failure = failure;
            PlacedOn = placedOn;
        }

        public static PlacementResult Ok(GridCell cell) => new(true, PlacementFailure.None, cell);
        public static PlacementResult Fail(PlacementFailure failure) => new(false, failure, null);
    }
}
