using System.Collections.Generic;
using System.Linq;
using _project.Scripts.Core.Enemy;
using _project.Scripts.Core.Events.Base;
using _project.Scripts.Core.Events.GameEventsModel;
using _project.Scripts.Core.Pathfinding.Main;
using _project.Scripts.Domain.Grid;
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
        List<GridCell> FindPathFrom(GridCell from);

        // مسیر فرضی اگه یه سل بسته بشه - بدون تغییر state.
        // برای پیش‌نمایش جایگذاری tower استفاده میشه.
        List<GridCell> SimulateBlockedPath(GridCell blocked);
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
            var startCell = cells.FirstOrDefault(c => c.Id == startId);
            var endCell = cells.FirstOrDefault(c => c.Id == endId);

            if (startCell == null || endCell == null)
            {
                _currentPath = new List<GridCell>();
                return;
            }

            // مسیر مرجع از start تا end (برای enemyهای جدید)
            var refPath = FindPath(startCell, endCell, cells);
            if (refPath == null || refPath.Count == 0)
            {
                _currentPath = new List<GridCell>();
                return;
            }

            _currentPath = refPath;

            // برای هر enemy alive، مسیر از سل فعلی خودش حساب بشه
            foreach (var enemy in enemyContainer.GetAliveEnemies())
            {
                var movement = enemy.Movement;
                if (movement == null) continue;

                var current = movement.CurrentCell;
                if (current == null) continue;

                var perEnemyPath = FindPathFrom(current);
                if (perEnemyPath == null || perEnemyPath.Count == 0)
                    continue;

                movement.SetPath(perEnemyPath);
            }
        }

        public List<GridCell> SimulateBlockedPath(GridCell blocked)
        {
            if (blocked == null) return null;
            var cells = grid.GetWalkableCells();
            // یه نسخه از سل‌ها بدون اون سل
            var filtered = new List<GridCell>(cells.Count);
            for (int i = 0; i < cells.Count; i++)
                if (cells[i].Id != blocked.Id) filtered.Add(cells[i]);

            var startCell = filtered.FirstOrDefault(c => c.Id == startId);
            var endCell = filtered.FirstOrDefault(c => c.Id == endId);
            if (startCell == null || endCell == null) return null;

            return FindPath(startCell, endCell, filtered);
        }

        public List<GridCell> FindPathFrom(GridCell from)
        {
            if (from == null) return null;
            var cells = grid.GetWalkableCells();

            // مطمئن شو سل فعلی enemy توی لیست walkable هست
            // (اگه روش تاور باشه نباید باشه ولی برای امنیت)
            if (!cells.Any(c => c.Id == from.Id))
                cells.Add(from);

            var endCell = cells.FirstOrDefault(c => c.Id == endId);
            if (endCell == null) return null;

            return FindPath(from, endCell, cells);
        }

        private List<GridCell> FindPath(GridCell startCell, GridCell targetCell, List<GridCell> walkableCell)
        {
            var startPathCell = new PathCell(startCell.Position.X, startCell.Position.Y);
            var endPathCell = new PathCell(targetCell.Position.X, targetCell.Position.Y);

            var allPathCells = walkableCell.Select(x => new PathCell(x.Position.X, x.Position.Y)).ToList();
            var path = pathfinder.FindPath(startPathCell, endPathCell, allPathCells);

            if (path == null) return null;

            var gridCellsPath = new List<GridCell>();
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
            return gridCellsPath;
        }
    }
}