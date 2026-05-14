using System;
using System.Collections.Generic;
using UnityEngine;

namespace _project.Scripts.Core.Economy
{
    [CreateAssetMenu(fileName = "WalletConfig", menuName = "Configs/Wallet Config")]
    public class WalletConfig : ScriptableObject
    {
        [Serializable]
        public struct StartingBalance
        {
            public CurrencyType type;
            [Min(0)] public int amount;
        }

        [SerializeField] private List<StartingBalance> startingBalances = new();

        public IReadOnlyList<StartingBalance> StartingBalances => startingBalances;
    }
}
