using System.Collections.Generic;
using _project.Scripts.Domain.Grid;
using _project.Scripts.Domain.Map;
using UnityEngine;

namespace _project.Scripts.Core.Map
{
    public interface IMapView{}
    public interface IMapFactory
    {
        IMapView CreateMap(MapConfig mapConfig);
    }
    public class MapInstaller
    {
        private readonly IMapProvider mapProvider;
        private readonly IMapFactory mapFactory;

        private IMapView spawnedMap;
        private GridData runtimeGridData;
        public MapInstaller(IMapProvider mapProvider, IMapFactory mapFactory)
        {
            this.mapProvider = mapProvider;
            this.mapFactory = mapFactory;
        }

        public MapInstallResult Install()
        {
            var config = mapProvider.GetSelectedMap();
            if (config == null)
            {
                Debug.LogError("[MapInstaller] No map selected!");
                return null;
            }

            // 1. Spawn map prefab
            spawnedMap = mapFactory.CreateMap(config);

            // 2. Clone grid data (تا اصل asset رو دست نزنیم)
            runtimeGridData = config.GridData.Clone();

            // 3. Build grid cells from data
            var gridCells = BuildGridCells(runtimeGridData);

            // 4. Resolve start/end cell IDs
            var endpoints = ResolveEndpoints(runtimeGridData, gridCells);

            return new MapInstallResult
            {
                MapInstance = spawnedMap,
                RuntimeGridData = runtimeGridData,
                GridCells = gridCells,
                Endpoints = endpoints
            };
        }

        private List<GridCell> BuildGridCells(GridData data)
        {
            var cells = new List<GridCell>();
            int id = 1;
            foreach (var worldPos in data.walkableNodes)
            {
                var gridPos = data.WorldToGrid(worldPos);
                var cell = new GridCell(
                    id++,
                    new GridPosition(gridPos.x, gridPos.y),
                    worldPos,
                    GridCellType.Walkable);
                cells.Add(cell);
            }
            return cells;
        }

        private (int startId, int endId) ResolveEndpoints(GridData data, List<GridCell> cells)
        {
            if (data.startPoints.Count == 0 || data.endPoints.Count == 0)
            {
                Debug.LogError("[MapInstaller] Map has no start/end points!");
                return (-1, -1);
            }

            var startWorld = data.startPoints[0];
            var endWorld = data.endPoints[0];

            // پیدا کردن نزدیک‌ترین cell به start/end (به جای equality دقیق با float)
            var startCell = FindNearestCell(cells, startWorld);
            var endCell = FindNearestCell(cells, endWorld);

            if (startCell == null || endCell == null)
            {
                Debug.LogError("[MapInstaller] Start/End not found!");
                return (-1, -1);
            }
            return (startCell.Id, endCell.Id);
        }

        private GridCell FindNearestCell(List<GridCell> cells, Vector3 worldPos)
        {
            GridCell nearest = null;
            float minDist = float.MaxValue;
            foreach (var cell in cells)
            {
                float dist = Vector3.SqrMagnitude(cell.WorldPosition - worldPos);
                if (dist < minDist) { minDist = dist; nearest = cell; }
            }
            return nearest;
        }
    }

    public class MapInstallResult
    {
        public IMapView MapInstance;
        public GridData RuntimeGridData;
        public List<GridCell> GridCells;
        public (int startId, int endId) Endpoints;
    }
}