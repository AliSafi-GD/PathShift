using _project.Scripts.Core.Cards;
using _project.Scripts.Core.Economy;
using _project.Scripts.Core.Tower;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _project.Scripts.UI.Cards
{
    // ویوی یک کارت داخل بار.
    // - tap → toggle انتخاب (مثل قبل)
    // - drag → انتخاب موقع شروع، رها کردن روی گرید → commit
    public class TowerCardView : MonoBehaviour,
        IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private Button button;
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text costText;
        [SerializeField] private GameObject selectedHighlight;
        [SerializeField] private CanvasGroup canvasGroup;     // اختیاری: موقع درگ alpha کم بشه
        [SerializeField] private float dragAlpha = 0.5f;

        private TowerCardData card;
        private ICardSelectionService selection;
        private IWallet wallet;
        private IPlacementCommitter committer;

        public TowerCardData Card => card;

        public void Bind(
            TowerCardData card,
            ICardSelectionService selection,
            IWallet wallet,
            IPlacementCommitter committer)
        {
            this.card = card;
            this.selection = selection;
            this.wallet = wallet;
            this.committer = committer;

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

        // ---- Click flow ----
        private void OnClick()
        {
            if (selection == null || card == null) return;
            if (selection.Current == card) selection.Clear();
            else selection.Select(card);
        }

        // ---- Drag flow ----
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (card == null || selection == null) return;
            if (wallet != null && !wallet.CanAfford(card.Cost)) return; // پول کافی نیست → اجازه‌ی درگ نده

            selection.Select(card);
            if (canvasGroup != null) canvasGroup.alpha = dragAlpha;
        }

        public void OnDrag(PointerEventData eventData)
        {
            // preview توسط PlacementPreviewController مدیریت میشه؛ اینجا کاری لازم نیست.
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (canvasGroup != null) canvasGroup.alpha = 1f;
            if (committer == null) { selection?.Clear(); return; }

            if (!committer.TryCommitAtMouse(out _, out var failure))
            {
                // commit شکست خورد → انتخاب رو پاک کن (کارت برمی‌گرده به deck)
                selection?.Clear();
                if (failure != PlacementFailure.NoCard)
                    Debug.Log($"[Card] Drop failed: {failure}");
            }
        }

        // ---- State refresh ----
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
