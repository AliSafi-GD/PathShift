using System.Collections.Generic;
using _project.Scripts.Core.Cards;
using _project.Scripts.Core.Economy;
using UnityEngine;
using VContainer;

namespace _project.Scripts.UI.Cards
{
    // بار پایین صفحه. کارت‌های دک رو می‌خونه و TowerCardView می‌سازه.
    public class TowerCardBarView : MonoBehaviour
    {
        [SerializeField] private TowerCardView cardViewPrefab;
        [SerializeField] private Transform cardsParent;

        private readonly List<TowerCardView> spawned = new();

        private DeckConfig deck;
        private ICardSelectionService selection;
        private IWallet wallet;

        [Inject]
        public void Construct(DeckConfig deck, ICardSelectionService selection, IWallet wallet)
        {
            this.deck = deck;
            this.selection = selection;
            this.wallet = wallet;
        }

        private void Start() => Build();

        private void Build()
        {
            foreach (var v in spawned) if (v != null) Destroy(v.gameObject);
            spawned.Clear();

            if (deck == null || cardViewPrefab == null || cardsParent == null) return;

            foreach (var card in deck.Cards)
            {
                if (card == null) continue;
                var view = Instantiate(cardViewPrefab, cardsParent);
                view.Bind(card, selection, wallet);
                spawned.Add(view);
            }
        }
    }
}
