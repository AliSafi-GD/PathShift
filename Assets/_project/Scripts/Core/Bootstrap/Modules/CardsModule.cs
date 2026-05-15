using _project.Scripts.Core.Cards;
using _project.Scripts.UI.Cards;
using VContainer;
using VContainer.Unity;

namespace _project.Scripts.Core.Bootstrap.Modules
{
    public static class CardsModule
    {
        public static void Install(
            IContainerBuilder builder,
            DeckConfig deckConfig,
            TowerCardBarView towerCardBarView)
        {
            builder.RegisterInstance(deckConfig);
            builder.Register<CardSelectionService>(Lifetime.Singleton).As<ICardSelectionService>();

            if (towerCardBarView != null)
                builder.RegisterComponent(towerCardBarView);
        }
    }
}
