using System;
using UnityEngine;

namespace _project.Scripts.Core.Economy
{
    // یک واحد هزینه: نوع ارز + مقدار.
    // بعداً اگه خواستی هزینه‌ی مرکب (چند ارز) داشته باشی، Cost[] استفاده کن یا CompositeCost بساز.
    [Serializable]
    public struct Cost
    {
        [SerializeField] private CurrencyType type;
        [SerializeField, Min(0)] private int amount;

        public CurrencyType Type => type;
        public int Amount => amount;

        public Cost(CurrencyType type, int amount)
        {
            this.type = type;
            this.amount = Mathf.Max(0, amount);
        }

        public bool IsFree => amount <= 0;

        public override string ToString() => $"{amount} {type}";
    }
}
