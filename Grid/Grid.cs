using UnityEngine;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{
    public static Grid Instance { get; private set; }

    public int width = 8;
    public int height = 8;

    private List<GridCell> gridCells = new List<GridCell>();
    private Canvas canvas;

    // Cell pozisyonları bir kez cache'lenir — her snap'te yeniden hesaplanmaz
    private Vector2[] cachedCellCanvasPositions;
    private bool cacheReady = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        canvas = FindFirstObjectByType<Canvas>();
    }

    public void AddCell(GridCell cell)
    {
        gridCells.Add(cell);
    }

    // CellSpawner FillGrid'i bitirince bunu çağırmalı
    public void BakePositions()
    {
        cachedCellCanvasPositions = new Vector2[gridCells.Count];
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        for (int i = 0; i < gridCells.Count; i++)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                RectTransformUtility.WorldToScreenPoint(null, gridCells[i].worldPosition),
                null,
                out cachedCellCanvasPositions[i]
            );
        }
        cacheReady = true;
    }

    // Mouse'un Canvas pozisyonuna en yakın cell'in Canvas pozisyonunu döndürür
    public Vector2? GetClosestCellPosition(Vector2 mouseCanvasPos)
    {
        if (!cacheReady || cachedCellCanvasPositions.Length == 0) return null;

        int closestIndex = 0;
        float closestDist = float.MaxValue;

        for (int i = 0; i < cachedCellCanvasPositions.Length; i++)
        {
            float dist = Vector2.SqrMagnitude(mouseCanvasPos - cachedCellCanvasPositions[i]);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestIndex = i;
            }
        }

        return cachedCellCanvasPositions[closestIndex];
    }
}