#if UNITY_EDITOR
using _project.Scripts.Editor.MapEditor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridGizmos))]
public class GridGizmosEditor : Editor
{
    private bool showStartPoints = false;
    private bool showEndPoints = false;
    private static GridGizmos activeGridGizmos = null;

    private void OnEnable()
    {
        activeGridGizmos = (GridGizmos)target;
        SceneView.duringSceneGui += OnSceneViewGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneViewGUI;
        activeGridGizmos = null;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GridGizmos gridGizmos = (GridGizmos)target;
        activeGridGizmos = gridGizmos;

        GUILayout.Space(10);
        if (GUILayout.Button("Bake Grid Data", GUILayout.Height(30)))
        {
            gridGizmos.BakeGridData();
            gridGizmos.currentSelectionMode = GridGizmos.SelectionMode.None;
        }

        GUILayout.Space(15);
        GUILayout.Label("Cell Selection for Start/End Points", EditorStyles.boldLabel);
        GUILayout.Label("روی سلول‌های گرید کلیک کنید تا انتخاب شوند", EditorStyles.miniLabel);

        GUILayout.Space(10);

        DrawPointSection(
            gridGizmos,
            ref showStartPoints,
            "Start Point Cells",
            gridGizmos.StartPoints,
            GridGizmos.SelectionMode.SelectingStart,
            Color.green,
            "Select Start Point Cells",
            "🎯 Selecting Start Points... (Click Cells)",
            "Clear All Start Cells"
        );

        GUILayout.Space(10);

        DrawPointSection(
            gridGizmos,
            ref showEndPoints,
            "End Point Cells",
            gridGizmos.EndPoints,
            GridGizmos.SelectionMode.SelectingEnd,
            Color.red,
            "Select End Point Cells",
            "🎯 Selecting End Points... (Click Cells)",
            "Clear All End Cells"
        );
    }

    // ───────────────────────────────────────────────────────────
    // Section drawing - یه helper که هم Start و هم End رو draw کنه
    // ───────────────────────────────────────────────────────────

    private void DrawPointSection(
        GridGizmos gridGizmos,
        ref bool foldout,
        string title,
        System.Collections.Generic.List<Vector3> points,
        GridGizmos.SelectionMode mode,
        Color activeColor,
        string buttonLabelInactive,
        string buttonLabelActive,
        string clearLabel)
    {
        foldout = EditorGUILayout.Foldout(foldout, title);
        if (!foldout) return;

        EditorGUI.indentLevel++;
        EditorGUILayout.LabelField($"Count: {points.Count}");

        bool isActive = gridGizmos.currentSelectionMode == mode;
        GUI.color = isActive ? activeColor : Color.white;

        if (GUILayout.Button(isActive ? buttonLabelActive : buttonLabelInactive,
            GUILayout.Height(30)))
        {
            gridGizmos.currentSelectionMode = isActive
                ? GridGizmos.SelectionMode.None
                : mode;
        }
        GUI.color = Color.white;

        // نمایش نقاط انتخاب‌شده
        for (int i = 0; i < points.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Point {i}: {FormatVector(points[i])}");

            if (GUILayout.Button("Remove", GUILayout.Width(70)))
            {
                points.RemoveAt(i);
                EditorUtility.SetDirty(gridGizmos);
                EditorGUILayout.EndHorizontal();
                break;
            }
            EditorGUILayout.EndHorizontal();
        }

        if (points.Count > 0 && GUILayout.Button(clearLabel, GUILayout.Height(20)))
        {
            points.Clear();
            EditorUtility.SetDirty(gridGizmos);
        }

        EditorGUI.indentLevel--;
    }

    private static string FormatVector(Vector3 v)
    {
        return $"({v.x:F2}, {v.y:F2}, {v.z:F2})";
    }

    // ───────────────────────────────────────────────────────────
    // Scene View click handling
    // ───────────────────────────────────────────────────────────

    private static void OnSceneViewGUI(SceneView sceneView)
    {
        if (activeGridGizmos == null) return;
        if (activeGridGizmos.currentSelectionMode == GridGizmos.SelectionMode.None) return;

        Event e = Event.current;
        if (e.type != EventType.MouseDown || e.button != 0) return;

        // Ray از ماوس
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

        // برخورد با ground plane (Y=0)
        if (Mathf.Approximately(ray.direction.y, 0f)) return;

        float t = -ray.origin.y / ray.direction.y;
        if (t <= 0) return;

        Vector3 hitPoint = ray.origin + ray.direction * t;

        // پیدا کردن نزدیک‌ترین node از NodeCenters
        Vector3 nearestNode = activeGridGizmos.GetNearestNode(hitPoint);

        // اگه NodeCenters خالیه (هنوز Draw نشده)، کاری نکن
        if (activeGridGizmos.NodeCenters.Count == 0)
        {
            Debug.LogWarning("[GridGizmosEditor] No nodes available. Make sure the scene is showing gizmos.");
            return;
        }

        // اضافه/حذف به لیست مربوطه
        var targetList = activeGridGizmos.currentSelectionMode == GridGizmos.SelectionMode.SelectingStart
            ? activeGridGizmos.StartPoints
            : activeGridGizmos.EndPoints;

        // toggle: اگه موجوده، حذف؛ وگرنه اضافه
        int existingIndex = FindNearbyIndex(targetList, nearestNode);
        if (existingIndex >= 0)
        {
            targetList.RemoveAt(existingIndex);
        }
        else
        {
            targetList.Add(nearestNode);
        }

        EditorUtility.SetDirty(activeGridGizmos);
        sceneView.Repaint();
        e.Use(); // جلوگیری از انتخاب objectهای دیگه
    }

    // مقایسه‌ی دو Vector3 با tolerance کوچیک (برای جلوگیری از مشکل floating-point)
    private static int FindNearbyIndex(System.Collections.Generic.List<Vector3> list, Vector3 target, float tolerance = 0.01f)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (Vector3.SqrMagnitude(list[i] - target) < tolerance * tolerance)
                return i;
        }
        return -1;
    }
}
#endif