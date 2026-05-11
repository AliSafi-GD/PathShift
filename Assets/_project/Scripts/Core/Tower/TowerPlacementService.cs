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

        public TowerPlacementService(
            IGrid grid,
            IPathService pathService,
            TowerFactory towerFactory,
            GridData gridData)
        {
            this.grid = grid;
            this.pathService = pathService;
            this.towerFactory = towerFactory;
            this.gridData = gridData;
        }

        public bool TryPlaceTower(Vector3 worldPosition, out GridCell placedOn)
        {
            placedOn = null;

            // 1) world → grid position
            var gp = gridData.WorldToGrid(worldPosition);
            var cell = grid.GetCell(new GridPosition(gp.x, gp.y));

            // 2) ولید بودن سل
            if (cell == null) return false;                                   // بیرون از گرید
            if (cell.GridCellType == GridCellType.Block) return false;        // قبلاً تاور داره
            if (cell.GridCellType == GridCellType.StartPoint) return false;
            if (cell.GridCellType == GridCellType.EndPoint) return false;

            // 3) شبیه‌سازی بلاک و چک کردن اینکه مسیر هنوز برقراره
            grid.SetWalkable(cell, false);
            pathService.Recalculate();
            var newPath = pathService.GetCurrentPath();

            if (newPath == null || newPath.Count == 0)
            {
                // مسیر بسته شد → rollback
                grid.SetWalkable(cell, true);
                pathService.Recalculate();
                return false;
            }

            // 4) ساخت تاور روی world position سل
            var towerView = towerFactory.CreateTower(null);
            towerView.transform.position = cell.WorldPosition;

            placedOn = cell;
            return true;
        }
    }
}