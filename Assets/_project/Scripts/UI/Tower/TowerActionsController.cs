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
        private Camera mainCamera;

        private PlacedTower current;

        [Inject]
        public void Construct(
            ITowerActionsService actions,
            ICardSelectionService cardSelection,
            IWallet wallet)
        {
            this.actions = actions;
            this.cardSelection = cardSelection;
            this.wallet = wallet;
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

        // از GameBootstrapper وقتی کارت انتخاب نشده صدا زده میشه.
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
        }

        public void ClosePopup()
        {
            current = null;
            if (popup != null) popup.Hide();
        }

        private void HandleSell()
        {
            if (current == null) return;
            actions.TrySell(current, out _);
            ClosePopup();
        }

        private void HandleUpgrade()
        {
            if (current == null) return;
            if (actions.TryUpgrade(current, out var failure))
            {
                // بعد از آپگرید popup رو دوباره بساز (تا قیمت/افوردابیلیتی آپدیت بشه)
                ShowPopup(current);
            }
            else
            {
                Debug.Log($"[Upgrade] Failed: {failure}");
            }
        }
    }
}
