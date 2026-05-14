using System;
using System.Collections.Generic;

namespace _project.Scripts.Core.Economy
{
    public class Wallet : IWallet
    {
        private readonly Dictionary<CurrencyType, int> balances = new();

        public event Action<CurrencyType, int> BalanceChanged;

        public Wallet(WalletConfig config = null)
        {
            if (config != null)
            {
                foreach (var entry in config.StartingBalances)
                    balances[entry.type] = entry.amount;
            }
        }

        public int Get(CurrencyType type)
            => balances.TryGetValue(type, out var v) ? v : 0;

        public bool CanAfford(Cost cost)
            => cost.IsFree || Get(cost.Type) >= cost.Amount;

        public bool TrySpend(Cost cost)
        {
            if (cost.IsFree) return true;
            if (!CanAfford(cost)) return false;

            var newBalance = Get(cost.Type) - cost.Amount;
            balances[cost.Type] = newBalance;
            BalanceChanged?.Invoke(cost.Type, newBalance);
            return true;
        }

        public void Add(CurrencyType type, int amount)
        {
            if (amount <= 0) return;
            var newBalance = Get(type) + amount;
            balances[type] = newBalance;
            BalanceChanged?.Invoke(type, newBalance);
        }
    }
}
