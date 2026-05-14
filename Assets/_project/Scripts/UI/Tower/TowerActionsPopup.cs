using System;
using _project.Scripts.Core.Cards;
using _project.Scripts.Core.Economy;
using _project.Scripts.Core.Tower;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _project.Scripts.UI.Tower
{
    // پنل با دو دکمه‌ی Sell / Upgrade. controller ست می‌کنه و نشون میده.
    public class TowerActionsPopup : MonoBehaviour
    {
        [SerializeField] private Button sellButton;
        [SerializeField] private TMP_Text sellLabel;
        [SerializeField] private Button upgradeButton;
        [SerializeField] private TMP_Text upgradeLabel;
        [SerializeField] private RectTransform root;

        public event Action OnSellClicked;
        public event Action OnUpgradeClicked;

        private void Awake()
        {
            if (root == null) root = (RectTransform)transform;
            if (sellButton != null) sellButton.onClick.AddListener(() => OnSellClicked?.Invoke());
            if (upgradeButton != null) upgradeButton.onClick.AddListener(() => OnUpgradeClicked?.Invoke());
            Hide();
        }

        public void Show(int sellRefund, CurrencyType refundCurrency,
                         bool upgradeAvailable, Cost upgradeCost, bool upgradeAffordable)
        {
            if (sellLabel != null) sellLabel.text = $"Sell\n+{sellRefund} {refundCurrency}";

            if (upgradeButton != null)
            {
                upgradeButton.gameObject.SetActive(upgradeAvailable);
                upgradeButton.interactable = upgradeAvailable && upgradeAffordable;
            }
            if (upgradeLabel != null)
                upgradeLabel.text = upgradeAvailable ? $"Upgrade\n-{upgradeCost.Amount} {upgradeCost.Type}" : "Max";

            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        // برای پوزیشن‌کردن روی نقطه‌ی world.
        public void SetScreenPosition(Vector2 screenPos)
        {
            if (root != null) root.position = screenPos;
        }
    }
}
