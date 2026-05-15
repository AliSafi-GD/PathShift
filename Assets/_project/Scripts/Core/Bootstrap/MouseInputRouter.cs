using _project.Scripts.Core.Cards;
using _project.Scripts.Core.Tower;
using _project.Scripts.UI.Tower;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace _project.Scripts.Core.Bootstrap
{
    // Routes world mouse clicks: if a card is selected, attempt placement;
    // otherwise forward the click to the tower-actions controller (popup).
    public class MouseInputRouter : MonoBehaviour
    {
        private IPlacementCommitter placementCommitter;
        private ICardSelectionService cardSelection;
        private TowerActionsController towerActionsController;

        private TestInput testInput;

        [Inject]
        public void Construct(
            IPlacementCommitter placementCommitter,
            ICardSelectionService cardSelection,
            TowerActionsController towerActionsController)
        {
            this.placementCommitter = placementCommitter;
            this.cardSelection = cardSelection;
            this.towerActionsController = towerActionsController;
        }

        private void Awake()
        {
            testInput = new TestInput();
            testInput.Gameplay.MouseClick.performed += OnMouseClick;
        }

        private void OnEnable() => testInput.Enable();
        private void OnDisable() => testInput.Disable();

        private void OnDestroy()
        {
            if (testInput == null) return;
            testInput.Gameplay.MouseClick.performed -= OnMouseClick;
            testInput.Dispose();
        }

        private void OnMouseClick(InputAction.CallbackContext context)
        {
            if (cardSelection?.Current != null)
            {
                placementCommitter?.TryCommitAtMouse();
                return;
            }

            towerActionsController?.HandleWorldClick();
        }
    }
}
