using _project.Scripts.Core.Bootstrap.Modules;
using _project.Scripts.Core.Enemy;
using _project.Scripts.Domain.Map;
using _project.Scripts.Core.Tower;
using _project.Scripts.Core.Wave;
using _project.Scripts.Core.Cards;
using _project.Scripts.Core.Economy;
using _project.Scripts.Presentation.View;
using _project.Scripts.UI.Cards;
using _project.Scripts.UI.Tower;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _project.Scripts.Core.Bootstrap
{
    public class GameLifetimeScope : LifetimeScope
    {
        [Header("Map")]
        [SerializeField] private HardcodedMapProvider mapProvider;
        [SerializeField] private MapFactory mapFactory;
        [SerializeField] private MainTowerFactory mainTowerFactory;
        [SerializeField] private PathVisualizer mainPathVisualizer;
        [SerializeField] private PathVisualizer previewPathVisualizer;

        [Header("Enemy / Waves")]
        [SerializeField] private EnemyFactory enemyFactory;
        [SerializeField] private LevelConfig levelConfig;

        [Header("Tower")]
        [SerializeField] private TowerFactory towerFactory;
        [SerializeField] private TowerAttackSystem towerAttackSystem;
        [SerializeField] private MortarProjectileFactory mortarProjectileFactory;
        [SerializeField] private PlacementPreviewController placementPreviewController;
        [SerializeField] private TowerRangeIndicator towerRangeIndicator;

        [Header("Economy / Cards")]
        [SerializeField] private WalletConfig walletConfig;
        [SerializeField] private DeckConfig deckConfig;
        [SerializeField] private TowerCardBarView towerCardBarView;

        [Header("UI")]
        [SerializeField] private TowerActionsController towerActionsController;

        [Header("Game")]
        [SerializeField] private GameBootstrapper gameBootstrapper;
        [SerializeField] private GameOverController gameOverController;

        protected override void Configure(IContainerBuilder builder)
        {
            MapModule.Install(builder, mapProvider, mapFactory, mainTowerFactory, mainPathVisualizer, previewPathVisualizer);
            EnemyModule.Install(builder, enemyFactory, levelConfig);
            TowerModule.Install(builder, towerFactory, towerAttackSystem, mortarProjectileFactory, placementPreviewController, towerRangeIndicator);
            EconomyModule.Install(builder, walletConfig);
            CardsModule.Install(builder, deckConfig, towerCardBarView);
            UIModule.Install(builder, towerActionsController);
            GameModule.Install(builder, gameBootstrapper, gameOverController);
        }
    }
}
