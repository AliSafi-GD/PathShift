using _project.Scripts.Core.Pathfinding;
using _project.Scripts.Domain.Grid;
using UnityEngine;

namespace _project.Scripts.Core.Tower
{
    public interface ITowerPlacementService
    {
        bool TryPlaceTower(Vector3 worldPosition, out GridCell placedOn);
    }

    public class TowerPlacementService : ITowerPlacementService
    {
        private readonly IGrid grid;
        private readonly IPathService pathService;
        private readonly TowerFactory towerFactory;
        private readonly GridData gridData;
        private readonly TowerAttackSystem attackSystem;

        private int nextId;

        public TowerPlacementService(
            IGrid grid,
            IPathService pathService,
            TowerFactory towerFactory,
            GridData gridData,
            TowerAttackSystem attackSystem)
        {
            this.grid = grid;
            this.pathService = pathService;
            this.towerFactory = towerFactory;
            this.gridData = gridData;
            this.attackSystem = attackSystem;
        }

        public bool TryPlaceTower(Vector3 worldPosition, out GridCell placedOn)
        {
            placedOn = null;

            var gp = gridData.WorldToGrid(worldPosition);
            var cell = grid.GetCell(new GridPosition(gp.x, gp.y));

            if (cell == null) return false;
            if (cell.GridCellType == GridCellType.Block) return false;
            if (cell.GridCellType == GridCellType.StartPoint) return false;
            if (cell.GridCellType == GridCellType.EndPoint) return false;

            grid.SetWalkable(cell, false);
            pathService.Recalculate();
            var newPath = pathService.GetCurrentPath();

            if (newPath == null || newPath.Count == 0)
            {
                grid.SetWalkable(cell, true);
                pathService.Recalculate();
                return false;
            }

            var (tower, _) = towerFactory.Create(cell.WorldPosition);
            tower.Id = ++nextId;
            attackSystem.Register(tower);

            placedOn = cell;
            return true;
        }
    }
}
