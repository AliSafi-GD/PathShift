using System.Collections.Generic;
using _project.Scripts.Domain.Interfaces;
using DG.Tweening;
using UnityEngine;

namespace _project.Scripts.Presentation.View
{
    // وقتی IHealth روی همون GameObject damage می‌خوره، یه shake + punch + color flash می‌زنه.
    // URP: از _BaseColor استفاده می‌کنه و با MaterialPropertyBlock کار می‌کنه (متریال instance نمی‌سازه).
    [DisallowMultipleComponent]
    public class DamageFlashAnimator : MonoBehaviour
    {
        [Header("Shake")]
        [SerializeField] private float shakeStrength = 0.15f;
        [SerializeField] private float shakeDuration = 0.2f;
        [SerializeField] private int shakeVibrato = 12;

        [Header("Punch scale")]
        [SerializeField] private float punchStrength = 0.12f;
        [SerializeField] private float punchDuration = 0.18f;

        [Header("Color flash")]
        [Tooltip("اگه خالی، همه‌ی Rendererهای فرزند flash می‌شن.")]
        [SerializeField] private Renderer[] targetRenderers;
        [SerializeField] private Color flashColor = Color.red;
        [SerializeField] private float flashDuration = 0.12f;

        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        private static readonly int ColorId = Shader.PropertyToID("_Color");

        private IHealth health;
        private float lastHealth;

        private Vector3 baseScale;
        private Vector3 baseLocalPos;

        // برای هر Renderer رنگ اصلی + MPB کش میشه
        private readonly List<RendererCache> rendererCaches = new();

        private struct RendererCache
        {
            public Renderer renderer;
            public Color originalColor;
            public MaterialPropertyBlock mpb;
            public bool useBaseColor; // true = _BaseColor (URP/HDRP)، false = _Color (Built-in)
        }

        private void Awake()
        {
            baseScale = transform.localScale;
            baseLocalPos = transform.localPosition;

            if (targetRenderers == null || targetRenderers.Length == 0)
                targetRenderers = GetComponentsInChildren<Renderer>(true);

            foreach (var r in targetRenderers)
            {
                if (r == null || r.sharedMaterial == null) continue;

                var mat = r.sharedMaterial;
                bool hasBase = mat.HasProperty(BaseColorId);
                bool hasColor = mat.HasProperty(ColorId);
                if (!hasBase && !hasColor) continue;

                var cache = new RendererCache
                {
                    renderer = r,
                    mpb = new MaterialPropertyBlock(),
                    useBaseColor = hasBase,
                    originalColor = hasBase ? mat.GetColor(BaseColorId) : mat.GetColor(ColorId),
                };
                rendererCaches.Add(cache);
            }
        }

        private void Start()
        {
            health = GetComponent<IHealth>();
            if (health == null) health = GetComponentInChildren<IHealth>();
            if (health == null) return;

            lastHealth = health.CurrentHealth;
            health.OnHealthChanged += OnHealthChanged;
        }

        private void OnDestroy()
        {
            if (health != null) health.OnHealthChanged -= OnHealthChanged;
            transform.DOKill();
        }

        private void OnHealthChanged(float current, float max)
        {
            if (current < lastHealth) PlayDamage();
            lastHealth = current;
        }

        private void PlayDamage()
        {
            transform.DOKill();
            transform.localScale = baseScale;
            transform.localPosition = baseLocalPos;

            transform.DOShakePosition(shakeDuration, shakeStrength, shakeVibrato, 90f, false, true)
                .OnComplete(() => transform.localPosition = baseLocalPos);

            transform.DOPunchScale(baseScale * punchStrength, punchDuration, 6, 0.5f);

            // رنگ flash
            for (int i = 0; i < rendererCaches.Count; i++)
                ApplyColor(rendererCaches[i], flashColor);

            DOVirtual.DelayedCall(flashDuration, () =>
            {
                for (int i = 0; i < rendererCaches.Count; i++)
                    ApplyColor(rendererCaches[i], rendererCaches[i].originalColor);
            });
        }

        private static void ApplyColor(RendererCache c, Color color)
        {
            if (c.renderer == null) return;
            c.renderer.GetPropertyBlock(c.mpb);
            if (c.useBaseColor) c.mpb.SetColor(BaseColorId, color);
            else c.mpb.SetColor(ColorId, color);
            c.renderer.SetPropertyBlock(c.mpb);
        }
    }
}
