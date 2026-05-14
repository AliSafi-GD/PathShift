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
        // تلاش برای کاشت کارت انتخاب‌شده در موقعیت فعلی موس.
        // اگر کارتی انتخاب نشده، یا روی UI هستیم، یا raycast هیت نمی‌کنه: false.
        bool TryCommitAtMouse(out GridCell placedOn, out PlacementFailure failure);
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

        public bool TryCommitAtMouse(out GridCell placedOn, out PlacementFailure failure)
        {
            placedOn = null;
            failure = PlacementFailure.None;

            var card = cardSelection?.Current;
            if (card == null) { failure = PlacementFailure.NoCard; return false; }

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            { failure = PlacementFailure.InvalidCell; return false; }

            var cam = Camera.main;
            if (cam == null || Mouse.current == null) return false;

            var screenPos = Mouse.current.position.ReadValue();
            var ray = cam.ScreenPointToRay(screenPos);
            if (!Physics.Raycast(ray, out var hit, 1000f)) return false;

            if (!placementService.TryPlaceTower(hit.point, card, out placedOn, out failure))
                return false;

            // commit موفق: انتخاب رو پاک کن و مسیر اصلی رو رفرش کن
            cardSelection.Clear();

            var newPath = pathService.GetCurrentPath();
            if (newPath != null && mainPathVisualizer != null)
            {
                var pts = new List<Vector3>(newPath.Count);
                for (int i = 0; i < newPath.Count; i++) pts.Add(newPath[i].WorldPosition);
                mainPathVisualizer.Show(pts);
            }
            return true;
        }
    }
}
