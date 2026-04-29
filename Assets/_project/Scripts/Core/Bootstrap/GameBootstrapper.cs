using System;
using System.Collections.Generic;
using System.Linq;
using _project.Scripts.Core.Context;
using _project.Scripts.Core.Enemy;
using _project.Scripts.Core.Events.Base;
using _project.Scripts.Core.Pathfinding.Application.AStar;
using _project.Scripts.Core.Pathfinding.Main;
using _project.Scripts.Domain.Grid;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _project.Scripts.Core.Bootstrap
{
    public class GameBootstrapper : MonoBehaviour
    {
        private ServiceLocator serviceLocator;
        [SerializeField] private CellView cellView;
        [SerializeField] private Transform parent;
        [SerializeField] private EnemyView enemyView;
        [SerializeField] private int start, end;
        private void Awake()
        {
            InitializeGameContext();
        }

        private void InitializeGameContext()
        {
            serviceLocator = new ServiceLocator();
            serviceLocator.Register<IPathfinder>(new AStarPathfinder());
            serviceLocator.Register<IEventBus>(new GameEventBus());
            serviceLocator.Register<IGrid>(new GridService(10,10));
            
            
            //create cell views
            var cellViewRegistry = new CellViewRegistry();
            GridFactory gridFactory = new GridFactory(cellView, parent);
            var gridCells = serviceLocator.Resolve<IGrid>().GetAllCells();
            var cellViews = gridFactory.CreateVisual(gridCells);
            cellViewRegistry.CellViews = new Dictionary<int, CellView>(cellViews);
            serviceLocator.Register<CellViewRegistry>(cellViewRegistry);
            
        }

        List<PathCell> path = new List<PathCell>();
        List<Vector3> vector3s = new List<Vector3>();
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                end = Random.Range(10, 100);
                vector3s.Clear();
                var cellViewRegistry = serviceLocator.Resolve<CellViewRegistry>();
                var pathfinder = serviceLocator.Resolve<IPathfinder>();
                var gridCells = serviceLocator.Resolve<IGrid>().GetAllCells();
                var startPathCell = new PathCell(gridCells[start].Position.X, gridCells[start].Position.Y);
                var endPathCell = new PathCell(gridCells[end].Position.X, gridCells[end].Position.Y);
                var allPathCells = gridCells.Select(x=>new PathCell(x.Position.X,x.Position.Y)).ToList();
                path = pathfinder.FindPath(startPathCell, endPathCell, allPathCells);
                
                foreach (var keyValuePair in cellViewRegistry.CellViews)
                {
                    keyValuePair.Value.GetComponent<Renderer>().material.color = Color.white;        

                }
                foreach (var pathCell in path)
                {
                    foreach (var gridCell in gridCells)
                    {
                        if (gridCell.Position.X == pathCell.X && gridCell.Position.Y == pathCell.Y)
                        {
                            cellViewRegistry.CellViews[gridCell.Id].GetComponent<Renderer>().material.color = Color.red;        
                            vector3s.Add(new Vector3(gridCell.Position.X, 1, gridCell.Position.Y));
                            break;
                        }
                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.V))
            {
                var enemy = Instantiate(enemyView);
                enemy.Move(vector3s);
            }
        }
    }
}