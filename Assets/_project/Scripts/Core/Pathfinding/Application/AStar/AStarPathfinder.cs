using System.Collections.Generic;
using System.Linq;
using _project.Scripts.Core.Pathfinding.Main;

namespace _project.Scripts.Core.Pathfinding.Application.AStar
{
    public class AStarPathfinder : IPathfinder
    {
        public List<PathCell> FindPath(
            PathCell startCell,
            PathCell endCell,
            List<PathCell> walkableCells)
        {
            var nodes = new Dictionary<PathCell, PathNode>();

            PathNode GetNode(PathCell c)
            {
                if (!nodes.ContainsKey(c))
                    nodes[c] = new PathNode(c);

                return nodes[c];
            }

            var openList = new List<PathNode>();
            var closedSet = new HashSet<PathNode>();

            var startNode = GetNode(startCell);
            var endNode = GetNode(endCell);

            startNode.GCost = 0;
            startNode.HCost = CalculateH(startCell, endCell);

            openList.Add(startNode);

            while (openList.Count > 0)
            {
                var current = openList.OrderBy(n => n.FCost).First();

                if (current == endNode)
                    return RetracePath(startNode, endNode);

                openList.Remove(current);
                closedSet.Add(current);

                foreach (var neighborCell in GetNeighbors(current.Cell, walkableCells))
                {
                    var neighborNode = GetNode(neighborCell);

                    if (closedSet.Contains(neighborNode))
                        continue;

                    int tentativeG = current.GCost + 10;

                    if (!openList.Contains(neighborNode) || tentativeG < neighborNode.GCost)
                    {
                        neighborNode.GCost = tentativeG;
                        neighborNode.HCost = CalculateH(neighborCell, endCell);
                        neighborNode.Parent = current;

                        if (!openList.Contains(neighborNode))
                            openList.Add(neighborNode);
                    }
                }
            }

            return null; // No path found
        }

        private IEnumerable<PathCell> GetNeighbors(PathCell cell, List<PathCell> walkable)
        {
            int x = cell.X;
            int y = cell.Y;

            // فقط ۴ جهت
            var candidates = new List<(int x, int y)>
            {
                (x+1, y),
                (x-1, y),
                (x, y+1),
                (x, y-1)
            };

            foreach (var c in candidates)
            {
                var neighbor = new PathCell(c.x, c.y);
                if (walkable.Contains(neighbor))
                    yield return neighbor;
            }
        }

        private List<PathCell> RetracePath(PathNode start, PathNode end)
        {
            var path = new List<PathCell>();
            var current = end;

            while (current != start)
            {
                path.Add(current.Cell);
                current = current.Parent;
            }

            path.Reverse();
            return path;
        }

        private int CalculateH(PathCell a, PathCell b)
        {
            int dx = System.Math.Abs(a.X - b.X);
            int dy = System.Math.Abs(a.Y - b.Y);
            return (dx + dy) * 10;
        }
    }
}