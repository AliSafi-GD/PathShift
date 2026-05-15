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
        // Non-mutating dry run for hover/preview UI.
        PlacementPreview Preview(Vector3 worldPosition, TowerCardData card);

        // Mutating commit: takes the cell, spends the cost, registers the tower.
        PlacementResult TryPlaceTower(Vector3 worldPosition, TowerCardData card);
    }

    public class TowerPlacementService : ITowerPlacementService
    {
        private readonly IGrid grid;
        private readonly IPathService pathService;
        private readonly ITowerFactory towerFactory;
        private readonly GridData gridData;
        private readonly IWallet wallet;
        private readonly IPlacedTowerRegistry placedRegistry;

        private int nextId;

        public TowerPlacementService(
            IGrid grid,
            IPathService pathService,
            ITowerFactory towerFactory,
            GridData gridData,
            IWallet wallet,
            IPlacedTowerRegistry placedRegistry)
        {
            this.grid = grid;
            this.pathService = pathService;
            this.towerFactory = towerFactory;
            this.gridData = gridData;
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

        public PlacementResult TryPlaceTower(Vector3 worldPosition, TowerCardData card)
        {
            var preview = Preview(worldPosition, card);
            if (!preview.IsValid)
                return PlacementResult.Fail(preview.Failure);

            var cell = preview.Cell;

            grid.SetWalkable(cell, false);
            pathService.Recalculate();

            if (!wallet.TrySpend(card.Cost))
            {
                // Race-condition guard: theoretical, roll back the grid change.
                grid.SetWalkable(cell, true);
                pathService.Recalculate();
                return PlacementResult.Fail(PlacementFailure.NotEnoughCurrency);
            }

            var (tower, view) = towerFactory.Create(cell.WorldPosition, card.TowerConfig);
            tower.Id = ++nextId;

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

            return PlacementResult.Ok(cell);
        }
    }
}
