using System.Collections.Generic;
using UnityEngine;

namespace _project.Scripts.Core.Cards
{
    // دکی که پلیر توی متا انتخاب کرده.
    // فعلاً به صورت ScriptableObject. وقتی متا اومد، این رو از سرویس متا پر می‌کنیم.
    [CreateAssetMenu(fileName = "DeckConfig", menuName = "Configs/Deck Config")]
    public class DeckConfig : ScriptableObject
    {
        [SerializeField] private List<TowerCardData> cards = new();

        public IReadOnlyList<TowerCardData> Cards => cards;
    }
}
