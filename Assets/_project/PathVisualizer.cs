using System.Collections.Generic;
using System.Linq;
using _project.Scripts.Core.Context;
using _project.Scripts.Core.Events.Base;
using _project.Scripts.Core.Events.GameEventsModel;
using _project.Scripts.Domain.Grid;
using UnityEngine;


public interface IMainPathVisualizer
{
    void Show(List<Vector3> path);
}

public interface IPreviewPathVisualizer
{
    void Show(List<Vector3> path);
}
public class PathVisualizer : MonoBehaviour , IGameEventListener<UpdatePath> ,IMainPathVisualizer,IPreviewPathVisualizer
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float yOffset = 0.1f;
    private CellViewRegistry cellViewRegistry;
    public void Constructor( CellViewRegistry cellViewRegistry)
    {
        this.cellViewRegistry = cellViewRegistry;
    }
    public void ShowPath(List<Vector3> path)
    {
        lineRenderer.positionCount = path.Count;

        for (int i = 0; i < path.Count; i++)
        {
            Vector3 pos = path[i];
            pos.y += yOffset;
            lineRenderer.SetPosition(i, pos);
        }

        lineRenderer.enabled = true;
    }

    public void Hide()
    {
        lineRenderer.enabled = false;
    }

    public void OnEventRaised(UpdatePath value)
    {
        Debug.Log("raised update path");
        var pathViews = new List<CellView>();

        foreach (var gridCell in value.Cells)
        {
            if (cellViewRegistry.TryGet(gridCell.Id, out var cellView))
            {
                pathViews.Add(cellView);
            }
            else
            {
                Debug.LogWarning($"CellView not found for GridCell id: {gridCell.Id}");
            }
        }
        // ShowPath(pathViews);
    }

    void IMainPathVisualizer.Show(List<Vector3> path)
    {
        ShowPath(path);
    }

    void IPreviewPathVisualizer.Show(List<Vector3> path)
    {
        ShowPath(path);
    }
}