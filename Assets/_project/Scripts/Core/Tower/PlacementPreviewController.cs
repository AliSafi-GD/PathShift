using System.Collections.Generic;
using _project.Scripts.Core.Cards;
using _project.Scripts.Domain.Grid;
using _project.Scripts.Presentation;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using VContainer;

namespace _project.Scripts.Core.Tower
{
    // وقتی پلیر یه کارت انتخاب کرده (یا داره درگ می‌کنه)، روی hover:
    //  - یه ghost (Quad سبز/قرمز) برای feedback اعتبار
    //  - مدل tower (previewPrefab کارت) برای پیش‌نمایش بصری
    //  - مسیر فرضی روی preview path visualizer
    public class PlacementPreviewController : MonoBehaviour
    {
        [Header("Ghost (validity feedback)")]
        [SerializeField] private Renderer ghostRenderer;
        [SerializeField] private Color validColor = new Color(0.2f, 1f, 0.2f, 0.55f);
        [SerializeField] private Color invalidColor = new Color(1f, 0.25f, 0.25f, 0.55f);
        [SerializeField] private float yOffset = 0.05f;

        [Header("Raycast")]
        [SerializeField] private float rayLength = 1000f;
        [SerializeField] private LayerMask raycastMask = ~0;

        [Header("Tower preview")]
        [Tooltip("پدر آبجکت‌های پیش‌نمایش tower. اگه خالی باشه، خودِ این آبجکت استفاده میشه.")]
        [SerializeField] private Transform towerPreviewParent;

        private ITowerPlacementService placementService;
        private ICardSelectionService cardSelection;
        private IPreviewPathVisualizer previewPath;
        private Camera mainCamera;

        private GameObject ghostRoot;
        private MaterialPropertyBlock mpb;
        private int colorPropId;

        private GameObject towerPreviewInstance;
        private TowerCardData towerPreviewForCard;

        private int lastCellId = int.MinValue;
        private bool lastValid;
        private bool hasGhost;

        [Inject]
        public void Construct(
            ITowerPlacementService placementService,
            ICardSelectionService cardSelection,
            IPreviewPathVisualizer previewPath)
        {
            this.placementService = placementService;
            this.cardSelection = cardSelection;
            this.previewPath = previewPath;
        }

        private void Awake()
        {
            mainCamera = Camera.main;
            mpb = new MaterialPropertyBlock();
            colorPropId = Shader.PropertyToID("_BaseColor");

            if (ghostRenderer != null)
            {
                ghostRoot = ghostRenderer.gameObject;
                ghostRoot.SetActive(false);
            }
            if (towerPreviewParent == null) towerPreviewParent = transform;
        }

        private void OnEnable()
        {
            if (cardSelection != null)
                cardSelection.SelectionChanged += OnSelectionChanged;
        }

        private void OnDisable()
        {
            if (cardSelection != null)
                cardSelection.SelectionChanged -= OnSelectionChanged;
            HideAll();
        }

        private void OnSelectionChanged(TowerCardData card)
        {
            EnsureTowerPreviewFor(card);
            InvalidateCache();
            if (card == null) HideAll();
        }

        private void Update()
        {
            var card = cardSelection?.Current;
            if (card == null)
            {
                HideAll();
                return;
            }

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                HideAll();
                return;
            }

            if (mainCamera == null) mainCamera = Camera.main;
            if (mainCamera == null || Mouse.current == null) return;

            var screenPos = Mouse.current.position.ReadValue();
            var ray = mainCamera.ScreenPointToRay(screenPos);
            if (!Physics.Raycast(ray, out var hit, rayLength, raycastMask))
            {
                HideAll();
                return;
            }

            var preview = placementService.Preview(hit.point, card);

            if (preview.Cell == null)
            {
                HideAll();
                return;
            }

            if (preview.Cell.Id != lastCellId || preview.IsValid != lastValid)
            {
                lastCellId = preview.Cell.Id;
                lastValid = preview.IsValid;
                ApplyVisuals(preview);
            }
            else
            {
                if (ghostRoot != null && !ghostRoot.activeSelf) ghostRoot.SetActive(true);
                if (towerPreviewInstance != null && !towerPreviewInstance.activeSelf) towerPreviewInstance.SetActive(true);
            }
        }

        private void ApplyVisuals(PlacementPreview p)
        {
            var basePos = p.Cell.WorldPosition;

            // Ghost (validity feedback)
            if (ghostRoot != null)
            {
                ghostRoot.SetActive(true);
                var pos = basePos;
                pos.y += yOffset;
                ghostRoot.transform.position = pos;
            }
            SetGhostColor(p.IsValid ? validColor : invalidColor);

            // Tower preview model
            if (towerPreviewInstance != null)
            {
                towerPreviewInstance.SetActive(true);
                towerPreviewInstance.transform.position = basePos;
            }

            // Preview path
            if (previewPath != null)
            {
                if (p.IsValid && p.Path != null && p.Path.Count > 0)
                {
                    var pts = new List<Vector3>(p.Path.Count);
                    for (int i = 0; i < p.Path.Count; i++) pts.Add(p.Path[i].WorldPosition);
                    previewPath.Show(pts);
                }
                else
                {
                    previewPath.Show(new List<Vector3>());
                }
            }

            hasGhost = true;
        }

        private void SetGhostColor(Color c)
        {
            if (ghostRenderer == null) return;
            ghostRenderer.GetPropertyBlock(mpb);
            mpb.SetColor(colorPropId, c);
            mpb.SetColor("_Color", c);
            ghostRenderer.SetPropertyBlock(mpb);
        }

        private void EnsureTowerPreviewFor(TowerCardData card)
        {
            if (towerPreviewForCard == card) return;

            if (towerPreviewInstance != null)
            {
                Destroy(towerPreviewInstance);
                towerPreviewInstance = null;
            }

            towerPreviewForCard = card;
            if (card == null || card.PreviewPrefab == null) return;

            towerPreviewInstance = Instantiate(card.PreviewPrefab, towerPreviewParent);
            // غیرفعالش می‌کنیم تا فقط موقع hover معتبر دیده بشه
            towerPreviewInstance.SetActive(false);
            // غیرفعال‌سازی هر Collider روی preview تا تو raycast دخالت نکنه
            foreach (var col in towerPreviewInstance.GetComponentsInChildren<Collider>(true))
                col.enabled = false;
        }

        private void HideAll()
        {
            if (ghostRoot != null) ghostRoot.SetActive(false);
            if (towerPreviewInstance != null) towerPreviewInstance.SetActive(false);
            if (previewPath != null && hasGhost) previewPath.Show(new List<Vector3>());
            hasGhost = false;
            InvalidateCache();
        }

        private void InvalidateCache()
        {
            lastCellId = int.MinValue;
            lastValid = false;
        }
    }
}
