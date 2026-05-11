using System;
using System.Collections.Generic;
using System.Linq;
using _project.Scripts.Core.Context;
using _project.Scripts.Core.Enemy;
using _project.Scripts.Core.Events.Base;
using _project.Scripts.Core.Events.GameEventsModel;
using _project.Scripts.Core.Pathfinding;
using _project.Scripts.Core.Pathfinding.Application.AStar;
using _project.Scripts.Core.Pathfinding.Main;
using _project.Scripts.Core.Spawner;
using _project.Scripts.Core.Tower;
using _project.Scripts.Core.Wave;
using _project.Scripts.Domain.Grid;
using _project.Scripts.Domain.Interfaces;
using _project.Scripts.Presentation;
using _project.Scripts.Presentation.View;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;
using Random = UnityEngine.Random;

namespace _project.Scripts.Core.Bootstrap
{
    public class GameBootstrapper : MonoBehaviour
    {
        IGrid grid;
        IPathService pathService;
        IEventBus eventBus;
        IWaveService waveService;
        CellViewRegistry cellViewRegistry;
        private IMainPathVisualizer mainPathVisualizer;
        [Inject]
        public void Construct(
            IGrid grid,
            IPathService pathService,
            IEventBus eventBus,
            IWaveService waveService,
            IMainPathVisualizer mainPathVisualizer)
        {
            this.grid = grid;
            this.pathService = pathService;
            this.eventBus = eventBus;
            this.waveService = waveService;
            this.mainPathVisualizer = mainPathVisualizer;
        }


        private void Start()
        {
            InitializeGame();
        }

        void InitializeGame()
        {
            var walkableCells = grid.GetWalkableCells();

            var currentPath = pathService.GetCurrentPath();
            
            List<Vector3> vects = new List<Vector3>();
            foreach (var gridCell in currentPath)
            {
                vects.Add(gridCell.WorldPosition);
            }
            mainPathVisualizer.Show(vects);
            //var startCell = walkableCells.Find(x => x.Id == start);
            //var endCell = walkableCells.Find(x => x.Id == end);

            //var path = pathService.FindPath(startCell, endCell, walkableCells);

            //eventBus.Raise(new UpdatePath(path));

            // _ = waveService.Start();
        }
        private TestInput testInput;

        private void Awake()
        {
            //InitializeGameContext();
            testInput = new TestInput();

            //testInput.Gameplay.FindPath.performed += OnFindPath;
            //testInput.Gameplay.SpawnEnemy.performed += OnSpawnEnemy;
            testInput.Gameplay.MouseClick.performed += MouseClick;
        }

        private void MouseClick(InputAction.CallbackContext obj)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000f))
            {
                // بررسی اینکه روی چی خورده
                var cellView = hit.collider.GetComponent<CellView>();
                if (cellView != null)
                {

                    var walkableCells = grid.GetWalkableCells();
                    walkableCells.Remove(cellView.cell);
                    //var startCell = walkableCells.Find(x => x.Id == start);
                    //var endCell = walkableCells.Find(x => x.Id == end);
                    //var gridCells = pathService.FindPath(startCell, endCell, walkableCells);

                    // if (gridCells != null)
                    // {
                    //     if (cellView.cell.IsWalkable)
                    //     {
                    //         cellView.Block();
                    //         var towerView = towerFactory.CreateTower(cellView.transform);
                    //         towerAttackSystem.towers.Add(new Tower.Tower(towerView.transform.position,
                    //             1,
                    //             new ClosestTargetPolicy(),
                    //             new CannonWeapon(projectileFactory,5)));
                    //     }
                    //     else
                    //     {
                    //         cellView.UnBlock();
                    //     }
                    //
                    //     gameEventBus.Raise(new UpdatePath(gridCells));
                        // enemySpawner.RecalculatePaths();
                    //}
                }
            }
        }

        private void OnEnable()
        {
            testInput.Enable();
        }

        private void OnDisable()
        {
            testInput.Disable();
        }
        

        private CellView cellViewSelected;
        [SerializeField] private int start;
        [SerializeField] private int end;

        public GameBootstrapper(IWaveService waveService)
        {
            this.waveService = waveService;
        }

        private void Update()
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000f))
            {
                var cellView = hit.collider.GetComponent<CellView>();
                if (cellView != null)
                {
                    cellViewSelected?.UnHighlight();
                    // cellViewSelected?.UnBlock();
                    cellViewSelected = cellView;
                    // cellViewSelected.Block();
                    cellView.Highlight();
                }
            }
            else
            {
                //cellViewSelected?.UnBlock();
                //previewPathVisualizer.Hide();
                cellViewSelected?.UnHighlight();
            }
        }

        private void OnSpawnEnemy(InputAction.CallbackContext context)
        {
            // var enemy = Instantiate(this.enemy);
            // var unityMovement = enemy.AddComponent<UnityMovement>();
            //
            // List<GridCell> cells = serviceLocator.Resolve<IGrid>().GetWalkableCells();
            //
            // var startGridCell = cells.First(x => x.Id == start);
            // var endGridCell = cells.First(x => x.Id == end);
            // var pathCells = serviceLocator.Resolve<NavigatorService>().FindPath(startGridCell, endGridCell, cells);
            // var _currentPath =  new List<GridCell>(pathCells);
            // unityMovement.SetPath(_currentPath);
            // unityMovement.Move();
            // enemy.Move(pathCellView);
        }

        private void OnDestroy()
        {
            waveService.Stop();
        }
    }
}