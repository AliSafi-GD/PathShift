using _project.Scripts.Domain.Grid;
using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace _project.Scripts.Editor.MapEditor
{
    [ExecuteAlways]
    [RequireComponent(typeof(LineRenderer))]
    public class GridGizmos : MonoBehaviour
    {
        [Header("Grid Settings")]
        public float cellSize = 1f;
        public Color gridColor = new Color(1f, 1f, 1f, 0.4f);
        public Color borderColor = Color.green;
        public Color nodeColor = Color.cyan;
        public float nodeRadius = 0.1f;

        [Header("Start/End Points")]
        public Color startPointColor = Color.green;
        public Color endPointColor = Color.red;
        public float pointRadius = 0.15f;
        public bool showStartEndPoints = true;

        [Header("Hole Settings")]
        public LineRenderer[] holeLineRenderers;
        public Color holeColor = Color.red;

        [Header("Grid Data")]
        public GridData gridData;

        // Editor-time state (هنوز هم لازم برای انتخاب کاربر قبل از Bake)
        public List<Vector3> NodeCenters { get; private set; } = new();
        public List<Vector3> StartPoints { get; private set; } = new();
        public List<Vector3> EndPoints { get; private set; } = new();

        public enum SelectionMode { None, SelectingStart, SelectingEnd }
        [HideInInspector] public SelectionMode currentSelectionMode = SelectionMode.None;

        private LineRenderer lr;
        private Vector3[] borderPoints;
        private List<Vector3[]> allHolePoints = new();

        // ───────────────────────────────────────────────────────────
        // Drawing
        // ───────────────────────────────────────────────────────────

        void OnDrawGizmos()
        {
            lr = GetComponent<LineRenderer>();
            if (lr == null) return;

            FetchPoints();
            if (borderPoints == null || borderPoints.Length < 3) return;

            DrawBorder();

            if (holeLineRenderers != null && holeLineRenderers.Length > 0)
            {
                FetchAllHolePoints();
                DrawAllHoles();
            }

            DrawGrid();

            if (showStartEndPoints)
                DrawStartEndPoints();
        }

        // ───────────────────────────────────────────────────────────
        // Bake
        // ───────────────────────────────────────────────────────────

        public void BakeGridData()
        {
            lr = GetComponent<LineRenderer>();
            FetchPoints();

            if (borderPoints == null || borderPoints.Length < 3)
            {
                Debug.LogError("Border points are invalid!");
                return;
            }

            if (holeLineRenderers != null && holeLineRenderers.Length > 0)
                FetchAllHolePoints();

            if (gridData == null)
            {
                Debug.LogError("GridData asset is not assigned!");
                return;
            }

            // محدوده گرید
            CalculateBounds(out float minX, out float maxX, out float minZ, out float maxZ);

            gridData.cellSize = cellSize;
            gridData.origin = transform.TransformPoint(new Vector3(minX, 0, minZ));
            gridData.width = Mathf.CeilToInt((maxX - minX) / cellSize);
            gridData.height = Mathf.CeilToInt((maxZ - minZ) / cellSize);

            gridData.walkableNodes.Clear();

            // پر کردن سلول‌های قابل عبور (فقط world position)
            for (float x = minX; x <= maxX; x += cellSize)
            {
                for (float z = minZ; z <= maxZ; z += cellSize)
                {
                    Vector3 center = new Vector3(x + cellSize * 0.5f, 0, z + cellSize * 0.5f);
                    if (!IsInsidePolygon(center)) continue;
                    if (IsInsideAnyHole(center)) continue;

                    Vector3 worldCenter = transform.TransformPoint(center);
                    gridData.walkableNodes.Add(worldCenter);
                }
            }

            // ذخیره start/end points (به‌صورت world position)
            gridData.startPoints = new List<Vector3>(StartPoints);
            gridData.endPoints = new List<Vector3>(EndPoints);

#if UNITY_EDITOR
            EditorUtility.SetDirty(gridData);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif

            Debug.Log($"Grid baked: {gridData.walkableNodes.Count} walkable nodes, " +
                      $"{gridData.startPoints.Count} start points, {gridData.endPoints.Count} end points");
        }

        // ───────────────────────────────────────────────────────────
        // Helpers - Drawing
        // ───────────────────────────────────────────────────────────

        void FetchPoints()
        {
            int count = lr.positionCount;
            borderPoints = new Vector3[count];
            lr.GetPositions(borderPoints);

            for (int i = 0; i < count; i++)
            {
                if (lr.useWorldSpace)
                    borderPoints[i] = transform.InverseTransformPoint(borderPoints[i]);
            }
        }

        void FetchAllHolePoints()
        {
            allHolePoints.Clear();

            foreach (var holeLR in holeLineRenderers)
            {
                if (holeLR == null) continue;

                int count = holeLR.positionCount;
                Vector3[] holePoints = new Vector3[count];
                holeLR.GetPositions(holePoints);

                for (int i = 0; i < count; i++)
                {
                    if (holeLR.useWorldSpace)
                    {
                        holePoints[i] = transform.InverseTransformPoint(holePoints[i]);
                    }
                    else
                    {
                        Vector3 worldPoint = holeLR.transform.TransformPoint(holePoints[i]);
                        holePoints[i] = transform.InverseTransformPoint(worldPoint);
                    }
                }

                allHolePoints.Add(holePoints);
            }
        }

        void DrawBorder()
        {
            Gizmos.color = borderColor;
            bool loop = lr.loop;
            int end = loop ? borderPoints.Length : borderPoints.Length - 1;

            for (int i = 0; i < end; i++)
            {
                int next = (i + 1) % borderPoints.Length;
                Gizmos.DrawLine(
                    transform.TransformPoint(borderPoints[i]),
                    transform.TransformPoint(borderPoints[next])
                );
            }
        }

        void DrawAllHoles()
        {
            Gizmos.color = holeColor;

            for (int h = 0; h < allHolePoints.Count; h++)
            {
                Vector3[] holePoints = allHolePoints[h];
                if (holePoints == null || holePoints.Length < 3) continue;

                LineRenderer holeLR = holeLineRenderers[h];
                bool loop = holeLR.loop;
                int end = loop ? holePoints.Length : holePoints.Length - 1;

                for (int i = 0; i < end; i++)
                {
                    int next = (i + 1) % holePoints.Length;
                    Gizmos.DrawLine(
                        transform.TransformPoint(holePoints[i]),
                        transform.TransformPoint(holePoints[next])
                    );
                }
            }
        }

        void DrawGrid()
        {
            CalculateBounds(out float minX, out float maxX, out float minZ, out float maxZ);

            Gizmos.color = gridColor;
            NodeCenters.Clear();

            for (float x = minX; x <= maxX; x += cellSize)
            {
                for (float z = minZ; z <= maxZ; z += cellSize)
                {
                    Vector3 center = new Vector3(x + cellSize * 0.5f, 0, z + cellSize * 0.5f);
                    if (!IsInsidePolygon(center)) continue;
                    if (IsInsideAnyHole(center)) continue;

                    Vector3 bl = new Vector3(x, 0, z);
                    Vector3 br = new Vector3(x + cellSize, 0, z);
                    Vector3 tl = new Vector3(x, 0, z + cellSize);
                    Vector3 tr = new Vector3(x + cellSize, 0, z + cellSize);

                    Gizmos.DrawLine(transform.TransformPoint(bl), transform.TransformPoint(br));
                    Gizmos.DrawLine(transform.TransformPoint(bl), transform.TransformPoint(tl));
                    Gizmos.DrawLine(transform.TransformPoint(br), transform.TransformPoint(tr));
                    Gizmos.DrawLine(transform.TransformPoint(tl), transform.TransformPoint(tr));

                    Vector3 worldCenter = transform.TransformPoint(center);
                    NodeCenters.Add(worldCenter);

                    Gizmos.color = nodeColor;
                    Gizmos.DrawSphere(worldCenter, nodeRadius);
                    Gizmos.color = gridColor;
                }
            }
        }

        void DrawStartEndPoints()
        {
            Gizmos.color = startPointColor;
            foreach (var worldPos in StartPoints)
                Gizmos.DrawSphere(worldPos, pointRadius);

            Gizmos.color = endPointColor;
            foreach (var worldPos in EndPoints)
                Gizmos.DrawSphere(worldPos, pointRadius);
        }

        void CalculateBounds(out float minX, out float maxX, out float minZ, out float maxZ)
        {
            minX = float.MaxValue; maxX = float.MinValue;
            minZ = float.MaxValue; maxZ = float.MinValue;

            foreach (var p in borderPoints)
            {
                if (p.x < minX) minX = p.x;
                if (p.x > maxX) maxX = p.x;
                if (p.z < minZ) minZ = p.z;
                if (p.z > maxZ) maxZ = p.z;
            }
        }

        // ───────────────────────────────────────────────────────────
        // Public utilities
        // ───────────────────────────────────────────────────────────

        public Vector3 GetNearestNode(Vector3 worldPos)
        {
            // اول از gridData (baked) استفاده کن
            if (gridData != null && gridData.walkableNodes.Count > 0)
                return FindNearest(gridData.walkableNodes, worldPos);

            // اگه bake نشده، از NodeCenters runtime استفاده کن
            if (NodeCenters.Count > 0)
                return FindNearest(NodeCenters, worldPos);

            return Vector3.zero;
        }

        private static Vector3 FindNearest(List<Vector3> nodes, Vector3 worldPos)
        {
            Vector3 nearest = nodes[0];
            float minDist = Vector3.SqrMagnitude(nodes[0] - worldPos);

            for (int i = 1; i < nodes.Count; i++)
            {
                float d = Vector3.SqrMagnitude(nodes[i] - worldPos);
                if (d < minDist)
                {
                    minDist = d;
                    nearest = nodes[i];
                }
            }

            return nearest;
        }

        // ───────────────────────────────────────────────────────────
        // Polygon tests
        // ───────────────────────────────────────────────────────────

        bool IsInsidePolygon(Vector3 point)
        {
            int count = borderPoints.Length;
            bool inside = false;
            int j = count - 1;

            for (int i = 0; i < count; i++)
            {
                float xi = borderPoints[i].x, zi = borderPoints[i].z;
                float xj = borderPoints[j].x, zj = borderPoints[j].z;

                bool intersect = ((zi > point.z) != (zj > point.z)) &&
                                 (point.x < (xj - xi) * (point.z - zi) / (zj - zi) + xi);
                if (intersect) inside = !inside;

                j = i;
            }

            return inside;
        }

        bool IsInsideAnyHole(Vector3 point)
        {
            foreach (var holePoints in allHolePoints)
            {
                if (IsInsideHole(point, holePoints))
                    return true;
            }
            return false;
        }

        bool IsInsideHole(Vector3 point, Vector3[] holePoints)
        {
            if (holePoints == null || holePoints.Length < 3) return false;

            int count = holePoints.Length;
            bool inside = false;
            int j = count - 1;

            for (int i = 0; i < count; i++)
            {
                float xi = holePoints[i].x, zi = holePoints[i].z;
                float xj = holePoints[j].x, zj = holePoints[j].z;

                bool intersect = ((zi > point.z) != (zj > point.z)) &&
                                 (point.x < (xj - xi) * (point.z - zi) / (zj - zi) + xi);
                if (intersect) inside = !inside;

                j = i;
            }
            return inside;
        }
    }
}