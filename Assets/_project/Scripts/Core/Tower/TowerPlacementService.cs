using System.Collections.Generic;
using _project.Scripts.Core.Cards;
using _project.Scripts.Core.Economy;
using _project.Scripts.Core.Pathfinding;
using _project.Scripts.Domain.Grid;
using UnityEngine;

namespace _project.Scripts.Core.Tower
{
    public enum PlacementFailure
    {
        None = 0,
        NoCard,
        InvalidCell,
        WouldBlockPath,
        NotEnoughCurrency,
    }

    // نتیجه‌ی یک پیش‌نمایش جایگذاری. غیرتخریبی.
    public struct PlacementPreview
    {
        public bool IsValid;            // همه چک‌ها پاس شد؟
        public PlacementFailure Failure;
        public GridCell Cell;           // سل هدف (می‌تونه null باشه اگه خارج گرید)
        public List<GridCell> Path;     // مسیر فرضی پس از جایگذاری (فقط اگه IsValid)
    }

    public interface ITowerPlacementService
    {
        // پیش‌نمایش غیرتخریبی - state تغییر نمی‌کنه.
        PlacementPreview Preview(Vector3 worldPosition, TowerCardData card);

        // اجرای واقعی - state تغییر می‌کنه و هزینه کم میشه.
        bool TryPlaceTower(Vector3 worldPosition, TowerCardData card, out GridCell placedOn, out PlacementFailure failure);
    }

    public class TowerPlacementService : ITowerPlacementService
    {
        private readonly IGrid grid;
        private readonly IPathService pathService;
        private readonly TowerFactory towerFactory;
        private readonly GridData gridData;
        private readonly TowerAttackSystem attackSystem;
        private readonly IWallet wallet;
        private readonly IPlacedTowerRegistry placedRegistry;

        private int nextId;

        public TowerPlacementService(
            IGrid grid,
            IPathService pathService,
            TowerFactory towerFactory,
            GridData gridData,
            TowerAttackSystem attackSystem,
            IWallet wallet,
            IPlacedTowerRegistry placedRegistry)
        {
            this.grid = grid;
            this.pathService = pathService;
            this.towerFactory = towerFactory;
            this.gridData = gridData;
            this.attackSystem = attackSystem;
            this.wallet = wallet;
            this.placedRegistry = placedRegistry;
        }

        public PlacementPreview Preview(Vector3 worldPosition, TowerCardData card)
        {
            var preview = new PlacementPreview();

            if (card == null || card.TowerConfig == null)
            {
                preview.Failure = PlacementFailure.NoCard;
                return preview;
            }

            var gp = gridData.WorldToGrid(worldPosition);
            var cell = grid.GetCell(new GridPosition(gp.x, gp.y));
            preview.Cell = cell;

            if (cell == null || cell.GridCellType != GridCellType.Walkable)
            {
                preview.Failure = PlacementFailure.InvalidCell;
                return preview;
            }

            // پول
            if (!wallet.CanAfford(card.Cost))
            {
                preview.Failure = PlacementFailure.NotEnoughCurrency;
                return preview;
            }

            // مسیر — non-mutating
            var simulated = pathService.SimulateBlockedPath(cell);
            if (simulated == null || simulated.Count == 0)
            {
                preview.Failure = PlacementFailure.WouldBlockPath;
                return preview;
            }

            preview.IsValid = true;
            preview.Path = simulated;
            return preview;
        }

        public bool TryPlaceTower(Vector3 worldPosition, TowerCardData card, out GridCell placedOn, out PlacementFailure failure)
        {
            placedOn = null;
            failure = PlacementFailure.None;

            var preview = Preview(worldPosition, card);
            if (!preview.IsValid)
            {
                failure = preview.Failure;
                return false;
            }

            var cell = preview.Cell;

            // commit
            grid.SetWalkable(cell, false);
            pathService.Recalculate();

            if (!wallet.TrySpend(card.Cost))
            {
                // race-condition guard: حالت تئوریک، rollback
                grid.SetWalkable(cell, true);
                pathService.Recalculate();
                failure = PlacementFailure.NotEnoughCurrency;
                return false;
            }

            var (tower, view) = towerFactory.Create(cell.WorldPosition, card.TowerConfig);
            tower.Id = ++nextId;
            attackSystem.Register(tower);

            placedRegistry?.Register(new PlacedTower
            {
                Tower = tower,
                View = view,
                Cell = cell,
                Card = card,
                UpgradeLevel = 0,
                TotalPaid = card.Cost.Amount,
                Currency = card.Cost.Type,
            });

            placedOn = cell;
            return true;
        }
    }
}
