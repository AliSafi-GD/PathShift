using UnityEngine;

namespace _project.Scripts.Presentation.View
{
    // دایره‌ی range برای tower با LineRenderer.
    // یه instance در صحنه؛ هرکسی بخواد Show(center, radius) رو صدا می‌زنه.
    [DisallowMultipleComponent]
    public class TowerRangeIndicator : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private int segments = 64;
        [SerializeField] private float yOffset = 0.05f;
        [SerializeField] private float width = 0.08f;
        [SerializeField] private Color color = new Color(0.3f, 1f, 0.4f, 0.6f);

        private void Awake()
        {
            if (lineRenderer == null) lineRenderer = GetComponent<LineRenderer>();
            if (lineRenderer == null) return;

            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
            lineRenderer.loop = true;
            lineRenderer.useWorldSpace = true;
            lineRenderer.positionCount = Mathf.Max(8, segments);
            gameObject.SetActive(false);
        }

        public void Show(Vector3 center, float radius)
        {
            if (lineRenderer == null) return;
            gameObject.SetActive(true);

            int n = Mathf.Max(8, segments);
            if (lineRenderer.positionCount != n) lineRenderer.positionCount = n;

            for (int i = 0; i < n; i++)
            {
                float a = (i / (float)n) * Mathf.PI * 2f;
                var p = center + new Vector3(Mathf.Cos(a) * radius, yOffset, Mathf.Sin(a) * radius);
                lineRenderer.SetPosition(i, p);
            }
        }

        public void SetColor(Color c)
        {
            color = c;
            if (lineRenderer == null) return;
            lineRenderer.startColor = c;
            lineRenderer.endColor = c;
        }

        public void Hide()
        {
            if (gameObject.activeSelf) gameObject.SetActive(false);
        }
    }
}
