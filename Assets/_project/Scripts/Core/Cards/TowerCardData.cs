using _project.Scripts.Core.Economy;
using _project.Scripts.Core.Tower;
using UnityEngine;

namespace _project.Scripts.Core.Cards
{
    // یک کارت = یک tower قابل ساخت در بازی.
    // داده‌ی نمایش (نام/آیکن) + هزینه + پیکربندی tower.
    [CreateAssetMenu(fileName = "TowerCard", menuName = "Configs/Tower Card")]
    public class TowerCardData : ScriptableObject
    {
        [Header("Display")]
        [SerializeField] private string displayName = "Tower";
        [SerializeField] private Sprite icon;

        [Header("Economy")]
        [SerializeField] private Cost cost = new Cost(CurrencyType.Coin, 50);

        [Header("Gameplay")]
        [SerializeField] private TowerConfig towerConfig;

        public string DisplayName => displayName;
        public Sprite Icon => icon;
        public Cost Cost => cost;
        public TowerConfig TowerConfig => towerConfig;
    }
}
