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
        // وقتی این editor فعال شود
        activeGridGizmos = (GridGizmos)target;
        SceneView.duringSceneGui += OnSceneViewGUI;
    }

    private void OnDisable()
    {
        // وقتی این editor غیرفعال شود
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

        // مدیریت نقاط شروع
        showStartPoints = EditorGUILayout.Foldout(showStartPoints, "Start Point Cells");
        if (showStartPoints)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField($"Count: {gridGizmos.StartPointCells.Count}");

            // دکمه انتخاب - Toggle برای Start
            bool isSelectingStart = gridGizmos.currentSelectionMode == GridGizmos.SelectionMode.SelectingStart;
            GUI.color = isSelectingStart ? Color.green : Color.white;
            if (GUILayout.Button(isSelectingStart ? "🎯 Selecting Start Points... (Click Cells)" : "Select Start Point Cells", 
                GUILayout.Height(30)))
            {
                if (isSelectingStart)
                {
                    // خاموش کن
                    gridGizmos.currentSelectionMode = GridGizmos.SelectionMode.None;
                }
                else
                {
                    // روشن کن
                    gridGizmos.currentSelectionMode = GridGizmos.SelectionMode.SelectingStart;
                }
            }
            GUI.color = Color.white;

            // نمایش سلول‌های انتخاب‌شده
            for (int i = 0; i < gridGizmos.StartPointCells.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"Cell {i}: {gridGizmos.StartPointCells[i]}");
                
                if (GUILayout.Button("Remove", GUILayout.Width(70)))
                {
                    gridGizmos.StartPointCells.RemoveAt(i);
                    EditorUtility.SetDirty(gridGizmos);
                }
                EditorGUILayout.EndHorizontal();
            }

            if (gridGizmos.StartPointCells.Count > 0 && GUILayout.Button("Clear All Start Cells", GUILayout.Height(20)))
            {
                gridGizmos.StartPointCells.Clear();
                EditorUtility.SetDirty(gridGizmos);
            }
            EditorGUI.indentLevel--;
        }

        GUILayout.Space(10);

        // مدیریت نقاط پایان
        showEndPoints = EditorGUILayout.Foldout(showEndPoints, "End Point Cells");
        if (showEndPoints)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField($"Count: {gridGizmos.EndPointCells.Count}");

            // دکمه انتخاب - Toggle برای End
            bool isSelectingEnd = gridGizmos.currentSelectionMode == GridGizmos.SelectionMode.SelectingEnd;
            GUI.color = isSelectingEnd ? Color.red : Color.white;
            if (GUILayout.Button(isSelectingEnd ? "🎯 Selecting End Points... (Click Cells)" : "Select End Point Cells", 
                GUILayout.Height(30)))
            {
                if (isSelectingEnd)
                {
                    // خاموش کن
                    gridGizmos.currentSelectionMode = GridGizmos.SelectionMode.None;
                }
                else
                {
                    // روشن کن
                    gridGizmos.currentSelectionMode = GridGizmos.SelectionMode.SelectingEnd;
                }
            }
            GUI.color = Color.white;

            // نمایش سلول‌های انتخاب‌شده
            for (int i = 0; i < gridGizmos.EndPointCells.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"Cell {i}: {gridGizmos.EndPointCells[i]}");
                
                if (GUILayout.Button("Remove", GUILayout.Width(70)))
                {
                    gridGizmos.EndPointCells.RemoveAt(i);
                    EditorUtility.SetDirty(gridGizmos);
                }
                EditorGUILayout.EndHorizontal();
            }

            if (gridGizmos.EndPointCells.Count > 0 && GUILayout.Button("Clear All End Cells", GUILayout.Height(20)))
            {
                gridGizmos.EndPointCells.Clear();
                EditorUtility.SetDirty(gridGizmos);
            }
            EditorGUI.indentLevel--;
        }
    }

    private static void OnSceneViewGUI(SceneView sceneView)
    {
        // اگر activeGridGizmos null باشد، کاری نکن
        if (activeGridGizmos == null || activeGridGizmos.gridData == null)
        {
            return;
        }

        // فقط اگر در حالت انتخاب باشی
        if (activeGridGizmos.currentSelectionMode == GridGizmos.SelectionMode.None)
        {
            return;
        }

        // دریافت event
        Event e = Event.current;

        if (e.type == EventType.MouseDown && e.button == 0)
        {
            // ریکاست Ray از ماوس
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

            // بررسی تقاطع با ground (Y = 0)
            if (ray.direction.y != 0)
            {
                float t = -ray.origin.y / ray.direction.y;
                Vector3 hitPoint = ray.origin + ray.direction * t;

                if (t > 0)
                {
                    // تبدیل به Grid Position
                    Vector2Int gridPos = activeGridGizmos.gridData.WorldToGrid(hitPoint);

                    // بررسی اینکه داخل گرید است یا نه
                    if (activeGridGizmos.gridData.IsWalkable(gridPos))
                    {
                        // حتما بررسی کن کدام mode است
                        if (activeGridGizmos.currentSelectionMode == GridGizmos.SelectionMode.SelectingStart)
                        {
                            if (!activeGridGizmos.StartPointCells.Contains(gridPos))
                            {
                                activeGridGizmos.StartPointCells.Add(gridPos);
                                EditorUtility.SetDirty(activeGridGizmos);
                            }
                            e.Use();
                        }
                        else if (activeGridGizmos.currentSelectionMode == GridGizmos.SelectionMode.SelectingEnd)
                        {
                            if (!activeGridGizmos.EndPointCells.Contains(gridPos))
                            {
                                activeGridGizmos.EndPointCells.Add(gridPos);
                                EditorUtility.SetDirty(activeGridGizmos);
                            }
                            e.Use();
                        }
                    }
                }
            }
        }
    }
}
#endif