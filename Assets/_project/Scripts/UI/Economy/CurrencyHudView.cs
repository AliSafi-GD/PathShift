using _project.Scripts.Core.Economy;
using TMPro;
using UnityEngine;
using VContainer;

namespace _project.Scripts.UI.Economy
{
    // یک لیبل ساده برای نمایش موجودی یک ارز. چندتاش رو می‌تونی توی صحنه بذاری.
    public class CurrencyHudView : MonoBehaviour
    {
        [SerializeField] private CurrencyType type = CurrencyType.Coin;
        [SerializeField] private TMP_Text label;
        [SerializeField] private string format = "{0}: {1}";

        private IWallet wallet;

        [Inject]
        public void Construct(IWallet wallet)
        {
            this.wallet = wallet;
        }

        private void Start()
        {
            if (wallet == null) return;
            wallet.BalanceChanged += OnChanged;
            Refresh(wallet.Get(type));
        }

        private void OnDestroy()
        {
            if (wallet != null) wallet.BalanceChanged -= OnChanged;
        }

        private void OnChanged(CurrencyType t, int amount)
        {
            if (t == type) Refresh(amount);
        }

        private void Refresh(int amount)
        {
            if (label != null) label.text = string.Format(format, type, amount);
        }
    }
}
