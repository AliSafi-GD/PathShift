using System.Collections.Generic;
using _project.Scripts.Core.Enemy;
using _project.Scripts.Core.Events.Base;
using _project.Scripts.Core.Pathfinding;
using _project.Scripts.Core.Pathfinding.Application.AStar;
using _project.Scripts.Core.Pathfinding.Main;
using _project.Scripts.Core.Spawner;
using _project.Scripts.Domain.Grid;
using _project.Scripts.Presentation;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _project.Scripts.Core.Bootstrap
{
    public class PathEndpoints : IPathEndpoints
    {
        public PathEndpoints(int startId, int endId)
        {
            StartId = startId;
            EndId = endId;
        }

        public int StartId { get; private set; }
        public int EndId { get; private set; }
    }
    public class GameLifetimeScope : LifetimeScope
    {
        [SerializeField] GameBootstrapper gameBootstrapper;
        [SerializeField] PathVisualizer mainPathVisualizer;
        [SerializeField] PathVisualizer previewPathVisualizer;
        [SerializeField] EnemyFactory enemyFactory;
        [SerializeField] private GridFactory gridFactory;
        
        [SerializeField] private int startCellIndex;
        [SerializeField] private int endCellIndex;
        
        IPathEndpoints pathEndpoints;
        
        
        //need a new service for get grid data
        [SerializeField] GridData gridData;

        private List<GridCell> GetGridCellFromData()
        {
            List<Vector2Int> clone = new List<Vector2Int>(gridData.walkableCells);
            List<GridCell> cells = new List<GridCell>();
            int id = 1;
            foreach (var vec in clone)
            {
                var cell = new GridCell(id++,new GridPosition(vec.x, vec.y),GridCellType.Walkable);
                cells.Add(cell);
            }
            return cells;
        }
        protected override void Configure(IContainerBuilder builder)
        {
            var start = gridData.walkableCells.Find(x =>
                x.x == gridData.startPointCells[0].x || x.y == gridData.startPointCells[0].y);
            var startIndex = gridData.walkableCells.IndexOf(start);
            var end = gridData.walkableCells.Find(x =>
                x.x == gridData.endPointCells[0].x || x.y == gridData.endPointCells[0].y);
            var endIndex = gridData.walkableCells.IndexOf(end);
            pathEndpoints = new PathEndpoints(startIndex, endIndex);
            // Core systems
            builder.Register<GridService>(Lifetime.Singleton)
                .As<IGrid>()
                .WithParameter(GetGridCellFromData);
            builder.RegisterInstance(pathEndpoints)
                .As<IPathEndpoints>();
            builder.Register<AStarPathfinder>(Lifetime.Singleton)
                .As<IPathfinder>();

            builder.Register<EnemyContainer>(Lifetime.Singleton);
            builder.Register<PathService>(Lifetime.Singleton)
                .As<IPathService>();

            builder.Register<GameEventBus>(Lifetime.Singleton)
                .As<IEventBus>();


            builder.Register<CellViewRegistry>(Lifetime.Singleton);

            // Enemy systems
            builder.Register<EnemySpawner>(Lifetime.Singleton)
                .As<IEnemySpawner>();
            
            builder.RegisterComponent(gameBootstrapper);

            builder.RegisterComponent<IMainPathVisualizer>(mainPathVisualizer);

            builder.RegisterComponent<IPreviewPathVisualizer>(previewPathVisualizer);

            builder.RegisterComponent(gridFactory);
            builder.RegisterComponent(enemyFactory);

            // builder.RegisterEntryPoint<GridPresenter>();
        }
    }

}