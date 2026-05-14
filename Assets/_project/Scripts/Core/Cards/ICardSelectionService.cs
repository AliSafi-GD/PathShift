using System;

namespace _project.Scripts.Core.Cards
{
    public interface ICardSelectionService
    {
        event Action<TowerCardData> SelectionChanged; // null یعنی deselect

        TowerCardData Current { get; }

        void Select(TowerCardData card);
        void Clear();
    }
}
