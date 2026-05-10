using System;
using System.Collections.Generic;
using UnityEngine;

public class GridData : ScriptableObject
{
    public float cellSize;
    public Vector3 origin;
    public int width;
    public int height;
    
    public List<Vector3> walkableNodes;
    public List<Vector3> startPoints;
    public List<Vector3> endPoints;

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

    public Vector3 GetStartPointWorld(int index)
    {
        if (index < 0 || index >= startPointCells.Count)
            return Vector3.zero;
        return GridToWorld(startPointCells[index]);
    }

    public Vector3 GetEndPointWorld(int index)
    {
        if (index < 0 || index >= endPointCells.Count)
            return Vector3.zero;
        return GridToWorld(endPointCells[index]);
    }

    public bool IsWalkable(Vector2Int gridPos)
    {
        return walkableCells.Contains(gridPos);
    }

    public List<Vector2Int> GetNeighbors(Vector2Int gridPos)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        Vector2Int[] directions = {
            new Vector2Int(0, 1),
            new Vector2Int(1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0)
        };

        foreach (var dir in directions)
        {
            Vector2Int neighbor = gridPos + dir;
            if (IsWalkable(neighbor))
                neighbors.Add(neighbor);
        }

        return neighbors;
    }

    public GridData Clone()
    {
        GridData clone = CreateInstance<GridData>();
        clone.cellSize = this.cellSize;
        clone.origin = this.origin;
        clone.width = this.width;
        clone.height = this.height;
        clone.walkableNodes = new List<Vector3>(this.walkableNodes);
        clone.walkableCells = new List<Vector2Int>(this.walkableCells);
        clone.startPointCells = new List<Vector2Int>(this.startPointCells);
        clone.endPointCells = new List<Vector2Int>(this.endPointCells);
        return clone;
    }
}