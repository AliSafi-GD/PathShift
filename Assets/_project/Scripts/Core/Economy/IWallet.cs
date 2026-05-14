using System;

namespace _project.Scripts.Core.Economy
{
    public interface IWallet
    {
        event Action<CurrencyType, int> BalanceChanged; // (نوع، مقدار جدید)

        int Get(CurrencyType type);
        bool CanAfford(Cost cost);
        bool TrySpend(Cost cost);
        void Add(CurrencyType type, int amount);
    }
}
