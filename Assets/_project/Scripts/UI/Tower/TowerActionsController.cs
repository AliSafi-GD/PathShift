using _project.Scripts.Core.Cards;
using _project.Scripts.Core.Economy;
using _project.Scripts.Core.Tower;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using VContainer;

namespace _project.Scripts.UI.Tower
{
    // روتر کلیک روی tower‌های ساخته‌شده.
    // - فقط وقتی کارتی انتخاب نشده فعاله (تا با placement تداخل نکنه).
    // - کلیک روی tower → popup. کلیک هرجا دیگه → popup بسته میشه.
    public class TowerActionsController : MonoBehaviour
    {
        [SerializeField] private TowerActionsPopup popup;
        [SerializeField] private Vector2 screenOffset = new Vector2(0, 60);
        [SerializeField] private float rayLength = 1000f;
        [SerializeField] private LayerMask raycastMask = ~0;

        private ITowerActionsService actions;
        private ICardSelectionService cardSelection;
        private IWallet wallet;
        private _project.Scripts.Presentation.View.TowerRangeIndicator rangeIndicator;
        private Camera mainCamera;

        private PlacedTower current;

        [Inject]
        public void Construct(
            ITowerActionsService actions,
            ICardSelectionService cardSelection,
            IWallet wallet,
            _project.Scripts.Presentation.View.TowerRangeIndicator rangeIndicator)
        {
            this.actions = actions;
            this.cardSelection = cardSelection;
            this.wallet = wallet;
            this.rangeIndicator = rangeIndicator;
        }

        private void Awake()
        {
            mainCamera = Camera.main;
            if (popup != null)
            {
                popup.OnSellClicked += HandleSell;
                popup.OnUpgradeClicked += HandleUpgrade;
            }
        }

        private void OnDestroy()
        {
            if (popup != null)
            {
                popup.OnSellClicked -= HandleSell;
                popup.OnUpgradeClicked -= HandleUpgrade;
            }
        }

        // Called by MouseInputRouter when no card is selected.
        public void HandleWorldClick()
        {
            if (cardSelection != null && cardSelection.Current != null) return;
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            if (mainCamera == null) mainCamera = Camera.main;
            if (mainCamera == null || Mouse.current == null) return;

            var screenPos = Mouse.current.position.ReadValue();
            var ray = mainCamera.ScreenPointToRay(screenPos);
            if (!Physics.Raycast(ray, out var hit, rayLength, raycastMask))
            {
                ClosePopup();
                return;
            }

            var placed = actions.TryResolveFromCollider(hit.collider);
            if (placed == null)
            {
                ClosePopup();
                return;
            }

            ShowPopup(placed);
        }

        private void ShowPopup(PlacedTower placed)
        {
            current = placed;
            if (popup == null) return;

            int refund = actions.GetSellRefund(placed);
            bool hasUpgrade = actions.TryGetNextUpgrade(placed, out var step);
            bool affordable = hasUpgrade && wallet.CanAfford(step.cost);

            popup.Show(refund, placed.Currency, hasUpgrade, step.cost, affordable);

            // پوزیشن popup بالای tower
            if (mainCamera != null && placed.View != null)
            {
                var sp = mainCamera.WorldToScreenPoint(placed.View.transform.position);
                popup.SetScreenPosition(new Vector2(sp.x, sp.y) + screenOffset);
            }

            // range indicator (آبی برای tower انتخاب‌شده)
            if (rangeIndicator != null && placed.View != null && placed.Tower != null)
            {
                rangeIndicator.SetColor(new Color(0.4f, 0.7f, 1f, 0.6f));
                rangeIndicator.Show(placed.View.transform.position, placed.Tower.Range);
            }
        }

        public void ClosePopup()
        {
            current = null;
            if (popup != null) popup.Hide();
            if (rangeIndicator != null) rangeIndicator.Hide();
        }

        private void HandleSell()
        {
            if (current == null) return;
            actions.TrySell(current);
            ClosePopup();
        }

        private void HandleUpgrade()
        {
            if (current == null) return;
            var result = actions.TryUpgrade(current);
            if (result.Success)
            {
                // Rebuild the popup so price/affordability reflect the new tier.
                ShowPopup(current);
            }
            else
            {
                Debug.Log($"[Upgrade] Failed: {result.Failure}");
            }
        }
    }
}
