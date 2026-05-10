using System.Collections.Generic;
using System.Linq;
using _project.Scripts.Core.Enemy;
using _project.Scripts.Core.Events.Base;
using _project.Scripts.Core.Events.GameEventsModel;
using _project.Scripts.Core.Pathfinding.Main;
using _project.Scripts.Domain.Grid;
using _project.Scripts.Presentation.View;
using UnityEngine;

namespace _project.Scripts.Core.Pathfinding
{
    public interface IPathEndpoints
    {
        int StartId { get; }
        int EndId { get; }
    }
    public interface IPathService
    {
        List<GridCell> GetCurrentPath();
        void Recalculate();
    }
    public class PathService : IPathService, IGameEventListener<GridCell>
    {
        private IPathfinder pathfinder;
        private readonly IGrid grid;
        private readonly int startId;
        private readonly int endId;
        private readonly EnemyContainer enemyContainer;

        private List<GridCell> _currentPath = new();

        public PathService(IGrid grid, 
            IPathEndpoints pathEndpoints, EnemyContainer container, IPathfinder pathfinder)
        {
            this.grid = grid;
            this.pathfinder = pathfinder;
            startId = pathEndpoints.StartId;
            endId = pathEndpoints.EndId;
            enemyContainer = container;

            Recalculate();
        }

        public List<GridCell> GetCurrentPath() => _currentPath;

        public void OnEventRaised(GridCell changedCell)
        {
            grid.SetWalkable(changedCell, false);
            Recalculate();
        }

        public void Recalculate()
        {
            var cells = grid.GetWalkableCells();
            var start = cells.First(c => c.Id == startId);
            var end = cells.First(c => c.Id == endId);

            var newPath = FindPath(start, end, cells);
            if (newPath == null || newPath.Count == 0)
                return;

            _currentPath = newPath;

            foreach (var enemy in enemyContainer.GetAliveEnemies())
                enemy.GetBehavior<UnityMovement>().SetPath(newPath);
        }
        private List<GridCell> FindPath(GridCell startCell,GridCell targetCell,List<GridCell> walkableCell)
        {
            var startPathCell = new PathCell(startCell.Position.X, startCell.Position.Y);
            var endPathCell = new PathCell(targetCell.Position.X, targetCell.Position.Y);

            var allPathCells = walkableCell.Select(x => new PathCell(x.Position.X, x.Position.Y)).ToList();
            var path = pathfinder.FindPath(startPathCell, endPathCell, allPathCells);
            
            List<GridCell> gridCellsPath = new List<GridCell>();
            
            if(path == null) return null;
            foreach (var pathCell in path)
            {
                foreach (var gridCell in walkableCell)
                {
                    if (gridCell.Position.X == pathCell.X && gridCell.Position.Y == pathCell.Y)
                    {
                        gridCellsPath.Add(gridCell);
                        break;
                    }
                }
            }
            return  gridCellsPath;
        }
    }
}