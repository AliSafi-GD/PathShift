using System.Collections.Generic;

namespace _project.Scripts.Core.Tower
{
    public interface IPlacedTowerRegistry
    {
        void Register(PlacedTower placed);
        void Unregister(PlacedTower placed);
        PlacedTower Find(TowerView view);
        IReadOnlyList<PlacedTower> All { get; }
    }

    public class PlacedTowerRegistry : IPlacedTowerRegistry
    {
        private readonly Dictionary<TowerView, PlacedTower> byView = new();
        private readonly List<PlacedTower> all = new();

        public IReadOnlyList<PlacedTower> All => all;

        public void Register(PlacedTower placed)
        {
            if (placed == null || placed.View == null) return;
            byView[placed.View] = placed;
            all.Add(placed);
        }

        public void Unregister(PlacedTower placed)
        {
            if (placed == null) return;
            if (placed.View != null) byView.Remove(placed.View);
            all.Remove(placed);
        }

        public PlacedTower Find(TowerView view)
            => view != null && byView.TryGetValue(view, out var p) ? p : null;
    }
}
