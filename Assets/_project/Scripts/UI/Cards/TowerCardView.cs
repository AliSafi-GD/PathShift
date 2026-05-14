using _project.Scripts.Core.Cards;
using _project.Scripts.Core.Economy;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _project.Scripts.UI.Cards
{
    // ویوی یک کارت داخل بار. الان uGUI ساده. بعداً قشنگش می‌کنیم.
    public class TowerCardView : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text costText;
        [SerializeField] private GameObject selectedHighlight;

        private TowerCardData card;
        private ICardSelectionService selection;
        private IWallet wallet;

        public TowerCardData Card => card;

        public void Bind(TowerCardData card, ICardSelectionService selection, IWallet wallet)
        {
            this.card = card;
            this.selection = selection;
            this.wallet = wallet;

            if (iconImage != null) iconImage.sprite = card.Icon;
            if (nameText != null) nameText.text = card.DisplayName;
            if (costText != null) costText.text = card.Cost.ToString();

            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(OnClick);
            }

            if (selection != null) selection.SelectionChanged += OnSelectionChanged;
            if (wallet != null) wallet.BalanceChanged += OnWalletChanged;

            RefreshAffordability();
            RefreshSelectedState();
        }

        private void OnDestroy()
        {
            if (selection != null) selection.SelectionChanged -= OnSelectionChanged;
            if (wallet != null) wallet.BalanceChanged -= OnWalletChanged;
        }

        private void OnClick()
        {
            if (selection == null || card == null) return;
            if (selection.Current == card) selection.Clear();
            else selection.Select(card);
        }

        private void OnSelectionChanged(TowerCardData _) => RefreshSelectedState();
        private void OnWalletChanged(CurrencyType _, int __) => RefreshAffordability();

        private void RefreshSelectedState()
        {
            if (selectedHighlight != null)
                selectedHighlight.SetActive(selection != null && selection.Current == card);
        }

        private void RefreshAffordability()
        {
            if (button == null || wallet == null || card == null) return;
            button.interactable = wallet.CanAfford(card.Cost);
        }
    }
}
