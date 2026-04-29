using System.Collections.Generic;

namespace _project.Scripts.Core.Pathfinding.Main
{
    public interface IPathfinder
    {
        List<PathCell> FindPath(
            PathCell startCell,
            PathCell endCell,
            List<PathCell> walkableCells);
    }
}