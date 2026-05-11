using System.Collections.Generic;
using _project.Scripts.Core.Events.Base;
using _project.Scripts.Core.Pathfinding;
using _project.Scripts.Core.Tower;
using _project.Scripts.Core.Wave;
using _project.Scripts.Domain.Grid;
using _project.Scripts.Presentation;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace _project.Scripts.Core.Bootstrap
{
    public class GameBootstrapper : MonoBehaviour
    {
        IGrid grid;
        IPathService pathService;
        IEventBus eventBus;
        IWaveService waveService;
        IMainPathVisualizer mainPathVisualizer;
        ITowerPlacementService towerPlacementService;   // ← جدید

        private TestInput testInput;

        [Inject]
        public void Construct(
            IGrid grid,
            IPathService pathService,
            IEventBus eventBus,
            IWaveService waveService,
            IMainPathVisualizer mainPathVisualizer,
            ITowerPlacementService towerPlacementService)   // ← جدید
        {
            this.grid = grid;
            this.pathService = pathService;
            this.eventBus = eventBus;
            this.waveService = waveService;
            this.mainPathVisualizer = mainPathVisualizer;
            this.towerPlacementService = towerPlacementService;
        }

        private void Awake()
        {
            testInput = new TestInput();
            testInput.Gameplay.MouseClick.performed += MouseClick;
        }

        private void Start()
        {
            InitializeGame();
        }

        private void OnEnable() => testInput.Enable();
        private void OnDisable() => testInput.Disable();

        private void OnDestroy()
        {
            waveService.Stop();
        }

        void InitializeGame()
        {
            var currentPath = pathService.GetCurrentPath();
            var vects = new List<Vector3>();
            foreach (var gridCell in currentPath)
                vects.Add(gridCell.WorldPosition);

            mainPathVisualizer.Show(vects);

            _ = waveService.Start();
        }

        private void MouseClick(InputAction.CallbackContext obj)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out RaycastHit hit, 1000f))
                return;

            if (towerPlacementService.TryPlaceTower(hit.point, out var placedCell))
            {
                Debug.Log($"[Tower] Placed on cell {placedCell.Id} at {placedCell.WorldPosition}");

                // مسیر بازسازی شده — visualizer رو هم آپدیت کن
                var newPath = pathService.GetCurrentPath();
                var pts = new List<Vector3>(newPath.Count);
                foreach (var c in newPath) pts.Add(c.WorldPosition);
                mainPathVisualizer.Show(pts);
            }
            else
            {
                Debug.Log("[Tower] Cannot place here");
            }
        }
    }
}