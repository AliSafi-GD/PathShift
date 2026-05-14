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
    // وقتی پلیر یه کارت انتخاب کرده، روی hover یه ghost سبز/قرمز نشون می‌ده
    // و مسیر فرضی نهایی رو روی preview path visualizer می‌کشه.
    public class PlacementPreviewController : MonoBehaviour
    {
        [Header("Ghost")]
        [SerializeField] private Renderer ghostRenderer;     // مش‌رندرر آبجکت ghost (مثلاً یه Quad/Cube بچه‌ی همین آبجکت)
        [SerializeField] private Color validColor = new Color(0.2f, 1f, 0.2f, 0.55f);
        [SerializeField] private Color invalidColor = new Color(1f, 0.25f, 0.25f, 0.55f);
        [SerializeField] private float yOffset = 0.05f;

        [Header("Raycast")]
        [SerializeField] private float rayLength = 1000f;
        [SerializeField] private LayerMask raycastMask = ~0;

        private ITowerPlacementService placementService;
        private ICardSelectionService cardSelection;
        private IPreviewPathVisualizer previewPath;
        private Camera mainCamera;

        private GameObject ghostRoot;     // پدر ghostRenderer (برای show/hide)
        private MaterialPropertyBlock mpb;
        private int colorPropId;

        // برای جلوگیری از محاسبه‌ی هر فریم
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
            colorPropId = Shader.PropertyToID("_BaseColor"); // URP. اگه built-in بود "_Color" هم set می‌کنیم پایین.

            if (ghostRenderer != null)
            {
                ghostRoot = ghostRenderer.gameObject;
                ghostRoot.SetActive(false);
            }
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

        private void OnSelectionChanged(TowerCardData _) => InvalidateCache();

        private void Update()
        {
            var card = cardSelection?.Current;
            if (card == null)
            {
                HideAll();
                return;
            }

            // روی UI بود → پنهان کن
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

            // هیچ سلی نگرفت → پنهان
            if (preview.Cell == null)
            {
                HideAll();
                return;
            }

            // فقط وقتی سل/وضعیت عوض شد بازکشی کن (preview path گرونه)
            if (preview.Cell.Id != lastCellId || preview.IsValid != lastValid)
            {
                lastCellId = preview.Cell.Id;
                lastValid = preview.IsValid;
                ApplyVisuals(preview);
            }
            else
            {
                // فقط موقعیت ghost رو ست کن (اگه قبلاً مخفی شده بوده)
                if (ghostRoot != null && !ghostRoot.activeSelf) ghostRoot.SetActive(true);
            }
        }

        private void ApplyVisuals(PlacementPreview p)
        {
            // ghost
            if (ghostRoot != null)
            {
                ghostRoot.SetActive(true);
                var pos = p.Cell.WorldPosition;
                pos.y += yOffset;
                ghostRoot.transform.position = pos;
            }
            SetGhostColor(p.IsValid ? validColor : invalidColor);

            // مسیر پیش‌نمایش
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
                    // مسیر invalid: یه لیست خالی بفرست (یا اگه Hide داشت می‌زدیم)
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
            mpb.SetColor("_Color", c); // fallback برای built-in
            ghostRenderer.SetPropertyBlock(mpb);
        }

        private void HideAll()
        {
            if (ghostRoot != null) ghostRoot.SetActive(false);
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
