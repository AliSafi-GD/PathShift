using System.Collections.Generic;
using _project.Scripts.Core.Cards;
using _project.Scripts.Core.Economy;
using _project.Scripts.Core.Pathfinding;
using _project.Scripts.Domain.Grid;
using _project.Scripts.Presentation;
using _project.Scripts.Presentation.View;
using UnityEngine;

namespace _project.Scripts.Core.Tower
{
    public enum TowerActionFailure
    {
        None,
        NotFound,
        NoUpgradeAvailable,
        NotEnoughCurrency,
    }

    public interface ITowerActionsService
    {
        // یه تاور رو از روی کلیک world پیدا کن (raycast باید بیرون انجام بشه؛ این از Collider/View استفاده می‌کنه).
        PlacedTower TryResolveFromCollider(Collider hit);

        // مقدار پولی که با حذف برمی‌گرده.
        int GetSellRefund(PlacedTower placed);

        bool TryGetNextUpgrade(PlacedTower placed, out TowerUpgradeStep step);

        bool TrySell(PlacedTower placed, out TowerActionFailure failure);
        bool TryUpgrade(PlacedTower placed, out TowerActionFailure failure);
    }

    public class TowerActionsService : ITowerActionsService
    {
        private readonly IPlacedTowerRegistry registry;
        private readonly IWallet wallet;
        private readonly IGrid grid;
        private readonly IPathService pathService;
        private readonly TowerAttackSystem attackSystem;
        private readonly TowerFactory towerFactory;
        private readonly IMainPathVisualizer mainPathVisualizer;

        public TowerActionsService(
            IPlacedTowerRegistry registry,
            IWallet wallet,
            IGrid grid,
            IPathService pathService,
            TowerAttackSystem attackSystem,
            TowerFactory towerFactory,
            IMainPathVisualizer mainPathVisualizer)
        {
            this.registry = registry;
            this.wallet = wallet;
            this.grid = grid;
            this.pathService = pathService;
            this.attackSystem = attackSystem;
            this.towerFactory = towerFactory;
            this.mainPathVisualizer = mainPathVisualizer;
        }

        public PlacedTower TryResolveFromCollider(Collider hit)
        {
            if (hit == null) return null;
            var view = hit.GetComponentInParent<TowerView>();
            return view == null ? null : registry.Find(view);
        }

        public int GetSellRefund(PlacedTower placed)
        {
            if (placed == null || placed.Card == null) return 0;
            return Mathf.FloorToInt(placed.TotalPaid * placed.Card.SellRefundRate);
        }

        public bool TryGetNextUpgrade(PlacedTower placed, out TowerUpgradeStep step)
        {
            step = default;
            if (placed == null || placed.Card == null) return false;
            return placed.Card.TryGetNextUpgrade(placed.UpgradeLevel, out step);
        }

        public bool TrySell(PlacedTower placed, out TowerActionFailure failure)
        {
            failure = TowerActionFailure.None;
            if (placed == null) { failure = TowerActionFailure.NotFound; return false; }

            int refund = GetSellRefund(placed);

            // attack system + registry
            attackSystem.Unregister(placed.Tower);
            registry.Unregister(placed);

            // grid + path
            if (placed.Cell != null)
            {
                grid.SetWalkable(placed.Cell, true);
                pathService.Recalculate();
            }

            // پول برگرده
            if (refund > 0) wallet.Add(placed.Currency, refund);

            // visual: shrink + spin، بعد destroy
            if (placed.View != null)
            {
                var go = placed.View.gameObject;
                JuiceFx.DespawnPopUp(go.transform, 0.35f, 0.6f, () => { if (go != null) Object.Destroy(go); });
            }

            RefreshMainPath();
            return true;
        }

        public bool TryUpgrade(PlacedTower placed, out TowerActionFailure failure)
        {
            failure = TowerActionFailure.None;
            if (placed == null) { failure = TowerActionFailure.NotFound; return false; }
            if (!TryGetNextUpgrade(placed, out var step))
            {
                failure = TowerActionFailure.NoUpgradeAvailable;
                return false;
            }
            if (!wallet.CanAfford(step.cost))
            {
                failure = TowerActionFailure.NotEnoughCurrency;
                return false;
            }
            if (step.towerConfig == null)
            {
                failure = TowerActionFailure.NoUpgradeAvailable;
                return false;
            }

            wallet.TrySpend(step.cost);

            // view جدید با کانفیگ آپگرید (اگه viewPrefab روی config ست شده باشه عوض میشه؛
            // وگرنه از defaultViewPrefab فاکتوری استفاده میشه = همون ظاهر قبلی).
            var worldPos = placed.View != null ? placed.View.transform.position : Vector3.zero;
            var rot = placed.View != null ? placed.View.transform.rotation : Quaternion.identity;
            var oldView = placed.View;

            var newView = towerFactory.SpawnView(worldPos, step.towerConfig);
            if (newView != null) newView.transform.rotation = rot;

            var oldTower = placed.Tower;
            var newTower = towerFactory.BuildTower(worldPos, step.towerConfig);
            newTower.Id = oldTower != null ? oldTower.Id : 0;

            attackSystem.Unregister(oldTower);
            attackSystem.Register(newTower);

            // مهم: registry رو با view جدید آپدیت کن قبل از Destroy کردن view قدیمی.
            registry.Unregister(placed);
            placed.Tower = newTower;
            placed.View = newView;
            placed.UpgradeLevel++;
            placed.TotalPaid += step.cost.Amount;
            registry.Register(placed);

            // view قدیمی رو با انیمیشن خداحافظی کن (sell/upgrade فرقی نداره visually).
            if (oldView != null && oldView != newView)
            {
                var oldGo = oldView.gameObject;
                JuiceFx.DespawnShrinkSpin(oldGo.transform, 0.25f, () => { if (oldGo != null) Object.Destroy(oldGo); });
            }

            // ارز ممکنه عوض شده باشه (مثلاً Coin → Gem برای آپگرید)؛ برای سادگی، refund همیشه با ارز اولیه میمونه.
            return true;
        }

        private void RefreshMainPath()
        {
            if (mainPathVisualizer == null) return;
            var newPath = pathService.GetCurrentPath();
            if (newPath == null) return;
            var pts = new List<Vector3>(newPath.Count);
            for (int i = 0; i < newPath.Count; i++) pts.Add(newPath[i].WorldPosition);
            mainPathVisualizer.Show(pts);
        }
    }
}
