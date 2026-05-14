using System.Collections.Generic;
using _project.Scripts.Core.Cards;
using _project.Scripts.Core.Events.Base;
using _project.Scripts.Core.Pathfinding;
using _project.Scripts.Core.Tower;
using _project.Scripts.Core.Wave;
using _project.Scripts.Domain.Grid;
using _project.Scripts.Presentation;
using _project.Scripts.UI.Tower;
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
        ICardSelectionService cardSelection;
        TowerActionsController towerActionsController;

        private TestInput testInput;

        [Inject]
        public void Construct(
            IGrid grid,
            IPathService pathService,
            IEventBus eventBus,
            IWaveService waveService,
            IMainPathVisualizer mainPathVisualizer,
            IPlacementCommitter placementCommitter,
            ICardSelectionService cardSelection,
            TowerActionsController towerActionsController)
        {
            this.grid = grid;
            this.pathService = pathService;
            this.eventBus = eventBus;
            this.waveService = waveService;
            this.mainPathVisualizer = mainPathVisualizer;
            this.placementCommitter = placementCommitter;
            this.cardSelection = cardSelection;
            this.towerActionsController = towerActionsController;
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
            // اگه کارتی انتخاب شده → تلاش به کاشت.
            // وگرنه → روتر اکشن tower (popup).
            if (cardSelection != null && cardSelection.Current != null)
            {
                placementCommitter?.TryCommitAtMouse(out _, out _);
                return;
            }

            towerActionsController?.HandleWorldClick();
        }
    }
}
