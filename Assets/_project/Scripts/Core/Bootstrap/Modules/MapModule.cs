using _project.Scripts.Core.Map;
using _project.Scripts.Core.Pathfinding;
using _project.Scripts.Core.Pathfinding.Application.AStar;
using _project.Scripts.Core.Pathfinding.Main;
using _project.Scripts.Core.Tower;
using _project.Scripts.Domain.Grid;
using _project.Scripts.Domain.Map;
using _project.Scripts.Presentation;
using VContainer;
using VContainer.Unity;

namespace _project.Scripts.Core.Bootstrap.Modules
{
    public static class MapModule
    {
        public static void Install(
            IContainerBuilder builder,
            HardcodedMapProvider mapProvider,
            MapFactory mapFactory,
            MainTowerFactory mainTowerFactory,
            PathVisualizer mainPathVisualizer,
            PathVisualizer previewPathVisualizer)
        {
            // Map providers/factories (scene components)
            builder.RegisterComponent<MainTowerFactory>(mainTowerFactory);
            builder.RegisterComponent<IMapProvider>(mapProvider);
            builder.RegisterComponent<IMapFactory>(mapFactory);

            // Map assembly (one-time install at startup)
            builder.Register<MapInstaller>(Lifetime.Singleton);
            builder.Register(c => c.Resolve<MapInstaller>().Install(), Lifetime.Singleton);
            builder.Register<IMapView>(c => c.Resolve<MapInstallResult>().MapInstance, Lifetime.Singleton);

            // MainTower depends on the assembled map view
            builder.Register(c =>
            {
                var factory = c.Resolve<MainTowerFactory>();
                return factory.Create(c.Resolve<IMapView>().GetMainTowerView());
            }, Lifetime.Singleton);

            // Grid
            builder.Register<GridService>(Lifetime.Singleton)
                .As<IGrid>()
                .WithParameter(c => c.Resolve<MapInstallResult>().GridCells);

            // Runtime grid data (needed by TowerPlacementService)
            builder.Register(c => c.Resolve<MapInstallResult>().RuntimeGridData, Lifetime.Singleton);

            // Pathfinding
            builder.Register<IPathEndpoints>(c =>
            {
                var endpoints = c.Resolve<MapInstallResult>().Endpoints;
                return new PathEndpoints(endpoints.startId, endpoints.endId);
            }, Lifetime.Singleton);
            builder.Register<AStarPathfinder>(Lifetime.Singleton).As<IPathfinder>();
            builder.Register<PathService>(Lifetime.Singleton).As<IPathService>();

            // Path visualizers
            builder.RegisterComponent<IMainPathVisualizer>(mainPathVisualizer);
            builder.RegisterComponent<IPreviewPathVisualizer>(previewPathVisualizer);
        }
    }
}
