using System;
using System.Collections.Generic;
using _project.Scripts.Core.Economy;
using _project.Scripts.Core.Tower;
using UnityEngine;

namespace _project.Scripts.Core.Cards
{
    [Serializable]
    public struct TowerUpgradeStep
    {
        [Tooltip("کانفیگ tower در این لول (damage / fireRate / range).")]
        public TowerConfig towerConfig;
        [Tooltip("هزینه‌ی این آپگرید.")]
        public Cost cost;
    }

    [CreateAssetMenu(fileName = "TowerCard", menuName = "Configs/Tower Card")]
    public class TowerCardData : ScriptableObject
    {
        [Header("Display")]
        [SerializeField] private string displayName = "Tower";
        [SerializeField] private Sprite icon;

        [Header("Economy")]
        [SerializeField] private Cost cost = new Cost(CurrencyType.Coin, 50);
        [Tooltip("نسبت بازگشت پول هنگام حذف tower (بین 0 تا 1).")]
        [Range(0f, 1f)]
        [SerializeField] private float sellRefundRate = 0.7f;

        [Header("Gameplay")]
        [SerializeField] private TowerConfig towerConfig;

        [Header("Upgrades")]
        [Tooltip("لیست آپگریدها به ترتیب. خالی = فقط لول پایه.")]
        [SerializeField] private List<TowerUpgradeStep> upgradeSteps = new();

        [Header("Preview")]
        [Tooltip("اختیاری. اگه ست بشه موقع پیش‌نمایش، این مدل روی سل هاور‌شده اسپان میشه.")]
        [SerializeField] private GameObject previewPrefab;

        public string DisplayName => displayName;
        public Sprite Icon => icon;
        public Cost Cost => cost;
        public float SellRefundRate => sellRefundRate;
        public TowerConfig TowerConfig => towerConfig;
        public IReadOnlyList<TowerUpgradeStep> UpgradeSteps => upgradeSteps;
        public GameObject PreviewPrefab => previewPrefab;

        // کانفیگ tower در یک لول مشخص. 0 = پایه.
        public TowerConfig GetConfigForLevel(int level)
        {
            if (level <= 0) return towerConfig;
            int idx = level - 1;
            if (idx >= upgradeSteps.Count) return upgradeSteps[upgradeSteps.Count - 1].towerConfig;
            return upgradeSteps[idx].towerConfig;
        }

        public bool TryGetNextUpgrade(int currentLevel, out TowerUpgradeStep step)
        {
            int nextIdx = currentLevel; // level 0 → نیاز به upgradeSteps[0]
            if (nextIdx >= 0 && nextIdx < upgradeSteps.Count)
            {
                step = upgradeSteps[nextIdx];
                return true;
            }
            step = default;
            return false;
        }
    }
}
