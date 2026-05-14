using _project.Scripts.Core.Cards;
using _project.Scripts.Core.Economy;
using _project.Scripts.Core.Enemy;
using _project.Scripts.Core.Events.Base;
using _project.Scripts.Core.Map;
using _project.Scripts.Core.Pathfinding;
using _project.Scripts.Core.Pathfinding.Application.AStar;
using _project.Scripts.Core.Pathfinding.Main;
using _project.Scripts.Core.Spawner;
using _project.Scripts.Core.Tower;
using _project.Scripts.Core.Wave;
using _project.Scripts.Domain.Grid;
using _project.Scripts.Domain.Map;
using _project.Scripts.Presentation;
using _project.Scripts.UI.Cards;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _project.Scripts.Core.Bootstrap
{
    public class GameLifetimeScope : LifetimeScope
    {
        [Header("Map")]
        [SerializeField] private HardcodedMapProvider mapProvider;

        [Header("Game")]
        [SerializeField] private GameBootstrapper gameBootstrapper;
        [SerializeField] private PathVisualizer mainPathVisualizer;
        [SerializeField] private PathVisualizer previewPathVisualizer;
        [SerializeField] private EnemyFactory enemyFactory;
        [SerializeField] private MapFactory mapFactory;
        [SerializeField] private MainTowerFactory mainTowerFactory;
        [SerializeField] private TowerFactory towerFactory;
        [SerializeField] private TowerAttackSystem towerAttackSystem;
        [SerializeField] private PlacementPreviewController placementPreviewController;
        [SerializeField] private GameOverController gameOverController;

        [SerializeField] private EnemySpawnConfig enemySpawnConfig;

        [Header("Economy / Cards")]
        [SerializeField] private WalletConfig walletConfig;
        [SerializeField] private DeckConfig deckConfig;
        [SerializeField] private TowerCardBarView towerCardBarView;

        protected override void Configure(IContainerBuilder builder)
        {
            // Map system - اول این، چون بقیه بهش وابسته‌ان
            builder.RegisterComponent<MainTowerFactory>(mainTowerFactory);
            builder.RegisterComponent<IMapProvider>(mapProvider);
            builder.RegisterComponent<IMapFactory>(mapFactory);
            builder.Register<MapInstaller>(Lifetime.Singleton);

            // Map result - با factory delegate ساخته میشه
            builder.Register(container =>
            {
                var installer = container.Resolve<MapInstaller>();
                return installer.Install();
            }, Lifetime.Singleton);

            builder.Register(container =>
            {
                var resolve = container.Resolve<MainTowerFactory>();
                var mainTower = resolve.Create(container.Resolve<IMapView>().GetMainTowerView());
                return mainTower;
            }, Lifetime.Singleton);
            // Grid - از نتیجه‌ی نصب نقشه
            builder.Register<GridService>(Lifetime.Singleton)
                .As<IGrid>()
                .WithParameter(container =>
                {
                    var result = container.Resolve<MapInstallResult>();
                    return result.GridCells;
                });

            // Path endpoints - از نتیجه‌ی نصب نقشه
            builder.Register<IPathEndpoints>(container =>
            {
                var result = container.Resolve<MapInstallResult>();
                return new PathEndpoints(result.Endpoints.startId, result.Endpoints.endId);
            }, Lifetime.Singleton);

            // Pathfinding
            builder.Register<AStarPathfinder>(Lifetime.Singleton).As<IPathfinder>();
            builder.Register<PathService>(Lifetime.Singleton).As<IPathService>();

            // Enemies
            builder.Register<EnemyContainer>(Lifetime.Singleton);
            builder.Register<EnemySpawner>(Lifetime.Singleton).As<IEnemySpawner>();
            builder.Register<WaveService>(Lifetime.Singleton).As<IWaveService>().WithParameter(enemySpawnConfig);
            // Events
            builder.Register<GameEventBus>(Lifetime.Singleton).As<IEventBus>();

            // Components
            builder.RegisterComponent(gameBootstrapper);
            builder.RegisterComponent<IMainPathVisualizer>(mainPathVisualizer);
            builder.RegisterComponent<IPreviewPathVisualizer>(previewPathVisualizer);
            builder.RegisterComponent(enemyFactory);
            builder.Register<IMapView>(container =>
            {
                var installer = container.Resolve<MapInstallResult>();
                return installer.MapInstance;
            },Lifetime.Singleton);

            // Tower system
            builder.RegisterComponent(towerFactory);
            builder.RegisterComponent(towerAttackSystem);

            // GridData runtime — needed by TowerPlacementService
            builder.Register(container =>
            {
                var result = container.Resolve<MapInstallResult>();
                return result.RuntimeGridData;
            }, Lifetime.Singleton);

            builder.Register<TowerPlacementService>(Lifetime.Singleton)
                .As<ITowerPlacementService>();

            if (placementPreviewController != null)
                builder.RegisterComponent(placementPreviewController);

            // Economy
            builder.Register<IWallet>(_ => new Wallet(walletConfig), Lifetime.Singleton);

            // Cards
            builder.RegisterInstance(deckConfig);
            builder.Register<CardSelectionService>(Lifetime.Singleton)
                .As<ICardSelectionService>();

            // UI (uGUI) — کارت بار و HUD اقتصاد. هرچی توی صحنه داری Inject میشه.
            if (towerCardBarView != null) builder.RegisterComponent(towerCardBarView);
            builder.RegisterBuildCallback(container =>
            {
                // Inject روی همه‌ی MonoBehaviour های صحنه که [Inject] دارن
                // (TowerCardBarView و CurrencyHudView های صحنه).
                // VContainer به صورت پیش‌فرض همینکار رو با AutoInjectGameObjects انجام میده،
                // ولی اگه نمی‌خوای روش حساب کنی، تو Awake خودشون هم می‌تونی resolve کنی.
            });

            // Game over UI
            builder.RegisterComponent(gameOverController);
        }
    }

    public class PathEndpoints : IPathEndpoints
    {
        public PathEndpoints(int startId, int endId)
        {
            StartId = startId;
            EndId = endId;
        }

        public int StartId { get; }
        public int EndId { get; }
    }
}
