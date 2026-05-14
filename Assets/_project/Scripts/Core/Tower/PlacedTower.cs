using _project.Scripts.Core.Cards;
using _project.Scripts.Core.Economy;
using _project.Scripts.Domain.Grid;

namespace _project.Scripts.Core.Tower
{
    // داده‌ی runtime یک تاور ساخته‌شده. سرویس‌های sell/upgrade ازش استفاده می‌کنن.
    public class PlacedTower
    {
        public Tower Tower;
        public TowerView View;
        public GridCell Cell;
        public TowerCardData Card;     // مرجع کارت اصلی (برای upgrade chain و refund rate)
        public int UpgradeLevel;       // 0 = base config، 1+ = ایندکس داخل upgradeSteps
        public int TotalPaid;          // مجموع هزینه‌ی پرداختی (base + همه آپگریدها)
        public CurrencyType Currency;  // ارز پرداخت‌شده — برگشت با همین ارز
    }
}
