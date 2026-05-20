using System.Collections.Generic;
using _project.Scripts.Core.Cards;
using _project.Scripts.Core.Pathfinding;
using _project.Scripts.Domain.Grid;
using _project.Scripts.Presentation;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace _project.Scripts.Core.Tower
{
    public interface IPlacementCommitter
    {
        // Attempt to place the selected card at the current mouse position.
        // Returns a failed result if no card is selected, the pointer is over UI,
        // or the world raycast misses.
        PlacementResult TryCommitAtMouse();
    }

    // منطق commit رو متمرکز می‌کنه تا چند ورودی (کلیک، رها کردن درگ) به یک رفتار برسن.
    public class PlacementCommitter : IPlacementCommitter
    {
        private readonly ITowerPlacementService placementService;
        private readonly ICardSelectionService cardSelection;
        private readonly IPathService pathService;
        private readonly IMainPathVisualizer mainPathVisualizer;

        public PlacementCommitter(
            ITowerPlacementService placementService,
            ICardSelectionService cardSelection,
            IPathService pathService,
            IMainPathVisualizer mainPathVisualizer)
        {
            this.placementService = placementService;
            this.cardSelection = cardSelection;
            this.pathService = pathService;
            this.mainPathVisualizer = mainPathVisualizer;
        }

        public PlacementResult TryCommitAtMouse()
        {
            var card = cardSelection?.Current;
            if (card == null)
                return PlacementResult.Fail(PlacementFailure.NoCard);

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return PlacementResult.Fail(PlacementFailure.InvalidCell);

            var cam = Camera.main;
            if (cam == null || Pointer.current == null)
                return PlacementResult.Fail(PlacementFailure.InvalidCell);

            var screenPos = Pointer.current.position.ReadValue();
            var ray = cam.ScreenPointToRay(screenPos);
            if (!Physics.Raycast(ray, out var hit, 1000f))
                return PlacementResult.Fail(PlacementFailure.InvalidCell);

            var result = placementService.TryPlaceTower(hit.point, card);
            if (!result.Success)
                return result;

            cardSelection.Clear();
            RefreshMainPath();
            return result;
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
