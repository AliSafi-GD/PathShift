namespace _project.Scripts.Core.Pathfinding
{
    public class PathEndpoints : IPathEndpoints
    {
        public PathEndpoints(int startId, int endId)
        {
            StartId = startId;
            EndId = endId;
        }

        public int StartId { get; }
        public int EndId { get; }
    }
}
