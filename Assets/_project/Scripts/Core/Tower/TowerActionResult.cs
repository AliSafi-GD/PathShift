namespace _project.Scripts.Core.Tower
{
    public readonly struct TowerActionResult
    {
        public readonly bool Success;
        public readonly TowerActionFailure Failure;

        private TowerActionResult(bool success, TowerActionFailure failure)
        {
            Success = success;
            Failure = failure;
        }

        public static TowerActionResult Ok() => new(true, TowerActionFailure.None);
        public static TowerActionResult Fail(TowerActionFailure failure) => new(false, failure);
    }
}
