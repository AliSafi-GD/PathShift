using System;

namespace _project.Scripts.Core.Cards
{
    public class CardSelectionService : ICardSelectionService
    {
        public event Action<TowerCardData> SelectionChanged;

        public TowerCardData Current { get; private set; }

        public void Select(TowerCardData card)
        {
            if (Current == card) return;
            Current = card;
            SelectionChanged?.Invoke(Current);
        }

        public void Clear()
        {
            if (Current == null) return;
            Current = null;
            SelectionChanged?.Invoke(null);
        }
    }
}
