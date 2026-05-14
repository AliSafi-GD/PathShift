using System.Collections.Generic;
using _project.Scripts.Core.Cards;
using _project.Scripts.Core.Economy;
using _project.Scripts.Core.Tower;
using UnityEngine;
using VContainer;

namespace _project.Scripts.UI.Cards
{
    public class TowerCardBarView : MonoBehaviour
    {
        [SerializeField] private TowerCardView cardViewPrefab;
        [SerializeField] private Transform cardsParent;

        private readonly List<TowerCardView> spawned = new();

        private DeckConfig deck;
        private ICardSelectionService selection;
        private IWallet wallet;
        private IPlacementCommitter committer;

        [Inject]
        public void Construct(
            DeckConfig deck,
            ICardSelectionService selection,
            IWallet wallet,
            IPlacementCommitter committer)
        {
            this.deck = deck;
            this.selection = selection;
            this.wallet = wallet;
            this.committer = committer;
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
                view.Bind(card, selection, wallet, committer);
                spawned.Add(view);
            }
        }
    }
}
