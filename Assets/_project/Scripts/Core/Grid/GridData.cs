using System.Collections.Generic;
using UnityEngine;

public class GridData : ScriptableObject
{
    public float cellSize;
    public Vector3 origin;
    public int width;
    public int height;

    public List<Vector3> walkableNodes = new List<Vector3>();
    public List<Vector3> startPoints = new List<Vector3>();
    public List<Vector3> endPoints = new List<Vector3>();

    // ───────────────────────────────────────────────────────────
    // Coordinate conversion utilities
    // (used by editor click-handling and A* pathfinding)
    // ───────────────────────────────────────────────────────────

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        Vector3 localPos = worldPos - origin;
        int x = Mathf.FloorToInt(localPos.x / cellSize);
        int z = Mathf.FloorToInt(localPos.z / cellSize);
        return new Vector2Int(x, z);
    }

    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        return origin + new Vector3(
            gridPos.x * cellSize + cellSize * 0.5f,
            0,
            gridPos.y * cellSize + cellSize * 0.5f
        );
    }

    // ───────────────────────────────────────────────────────────
    // Cloning
    // ───────────────────────────────────────────────────────────

    public GridData Clone()
    {
        GridData clone = CreateInstance<GridData>();
        clone.cellSize = this.cellSize;
        clone.origin = this.origin;
        clone.width = this.width;
        clone.height = this.height;
        clone.walkableNodes = new List<Vector3>(this.walkableNodes);
        clone.startPoints = new List<Vector3>(this.startPoints);
        clone.endPoints = new List<Vector3>(this.endPoints);
        return clone;
    }
}