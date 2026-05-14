using System.Collections.Generic;
using _project.Scripts.Core.Cards;
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
        IPlacementCommitter placementCommitter;

        private TestInput testInput;

        [Inject]
        public void Construct(
            IGrid grid,
            IPathService pathService,
            IEventBus eventBus,
            IWaveService waveService,
            IMainPathVisualizer mainPathVisualizer,
            IPlacementCommitter placementCommitter)
        {
            this.grid = grid;
            this.pathService = pathService;
            this.eventBus = eventBus;
            this.waveService = waveService;
            this.mainPathVisualizer = mainPathVisualizer;
            this.placementCommitter = placementCommitter;
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
            // click flow: کارت انتخاب شده + کلیک روی گرید → کاشت.
            // drag flow: TowerCardView.OnEndDrag مستقل از این کار می‌کنه.
            if (placementCommitter == null) return;

            if (placementCommitter.TryCommitAtMouse(out var placedCell, out var failure))
                Debug.Log($"[Tower] Placed on cell {placedCell.Id}");
            else if (failure != PlacementFailure.NoCard)
                Debug.Log($"[Tower] Cannot place here: {failure}");
        }
    }
}
