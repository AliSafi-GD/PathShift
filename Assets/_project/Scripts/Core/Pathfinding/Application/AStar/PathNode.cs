using _project.Scripts.Core.Pathfinding.Main;

namespace _project.Scripts.Core.Pathfinding.Application.AStar
{
    public class PathNode
    {
        public readonly PathCell Cell;
        public int GCost;
        public int HCost;
        public int FCost => GCost + HCost;
        public PathNode Parent;

        public PathNode(PathCell cell)
        {
            Cell = cell;
        }
    }
}