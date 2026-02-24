using UnityEngine;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{
    public static Grid Instance { get; private set; }

    public int width = 8;
    public int height = 8;

    private List<GridCell> gridCells = new List<GridCell>();
    public GridCell[,] gridCells2D = new GridCell[8,8];
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
        Debug.Log("Added cell at grid pos: " + cell.gridPosition);
        gridCells.Add(cell);
        gridCells2D[cell.gridPosition.x, cell.gridPosition.y] = cell;
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

    // Mouse pozisyonuna en yakın BOŞ cell'i döndürür
    public Vector2? GetClosestCellPosition(Vector2 mouseCanvasPos)
    {
        if (!cacheReady || cachedCellCanvasPositions.Length == 0) return null;

        int closestIndex = -1;
        float closestDist = float.MaxValue;

        for (int i = 0; i < cachedCellCanvasPositions.Length; i++)
        {
            if (gridCells[i].isOccupied) continue; // Dolu cell'i atla

            float dist = Vector2.SqrMagnitude(mouseCanvasPos - cachedCellCanvasPositions[i]);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestIndex = i;
            }
        }

        if (closestIndex == -1) return null;
        return cachedCellCanvasPositions[closestIndex];
    }

    // Canvas pozisyonuna en yakın cell'in index'ini döndürür
    public int GetClosestCellIndex(Vector2 canvasPos)
    {
        if (!cacheReady) return -1;

        int closestIndex = -1;
        float closestDist = float.MaxValue;

        for (int i = 0; i < cachedCellCanvasPositions.Length; i++)
        {
            float dist = Vector2.SqrMagnitude(canvasPos - cachedCellCanvasPositions[i]);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestIndex = i;
            }
        }
        return closestIndex;
    }

    public bool CanOccupyCells(List<Vector2> tileCanvasPositions, List<Tile> tiles, float snapThreshold)
    {
        List<(int idx, Tile tile)> candidates = new List<(int, Tile)>();

        for(int i = 0 ; i < tileCanvasPositions.Count ; i++)
        {
            int idx = GetClosestCellIndex(tileCanvasPositions[i]);
            if(idx == -1) return false;

            float dist = Vector2.Distance(tileCanvasPositions[i], cachedCellCanvasPositions[idx]);
            if (dist > snapThreshold) return false;

            if (gridCells[idx].isOccupied) return false;

            candidates.Add((idx, tiles[i]));
        }

        return true;
    }

    // Bu şekil, tahta üzerinde herhangi bir pozisyona sığıyor mu?
    public bool CanShapeFitAnywhere(BlockShape shape)
    {
        Vector2Int[] offsets = BlockShapeData.Offsets[shape];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                bool fits = true;
                foreach (var offset in offsets)
                {
                    int nx = x + offset.x;
                    int ny = y + offset.y;
                    if (nx < 0 || nx >= width || ny < 0 || ny >= height || gridCells2D[nx, ny].isOccupied)
                    {
                        fits = false;
                        break;
                    }
                }
                if (fits) return true;
            }
        }
        return false;
    }
    // Belirtilen canvas pozisyonlarındaki cell'leri dolu işaretle
    // snapThreshold: her tile'ın cell'e maksimum uzaklığı (piksel)
    public bool OccupyCells(List<Vector2> tileCanvasPositions, List<Tile> tiles, float snapThreshold)
    {
        List<(int idx, Tile tile)> candidates = new List<(int, Tile)>();

        for (int i = 0; i < tileCanvasPositions.Count; i++)
        {
            int idx = GetClosestCellIndex(tileCanvasPositions[i]);
            if(idx == -1) return false;
            candidates.Add((idx, tiles[i]));
        }

        foreach (var (idx, tile) in candidates)
            gridCells[idx].SetOccupied(tile);

        return true;
    }

    public void ClearRow(int y)
    {
        if (ParticleSpawner.Instance == null)
            Debug.LogWarning("ParticleSpawner.Instance is null — sahneye ParticleSpawner ekli mi?");

        for (int x = 0; x < width; x++)
        {
            GridCell cell = gridCells2D[x, y];
            if (cell.occupyingTile != null)
            {
                if (ParticleSpawner.Instance != null)
                    ParticleSpawner.Instance.Spawn(cell.occupyingTile.ColorIndex, cell.occupyingTile);
                Destroy(cell.occupyingTile.gameObject);
            }
            cell.SetOccupied(null);
        }
    }

    public void ClearColumn(int x)
    {
        if (ParticleSpawner.Instance == null)
            Debug.LogWarning("ParticleSpawner.Instance is null — sahneye ParticleSpawner ekli mi?");

        for (int y = 0; y < height; y++)
        {
            GridCell cell = gridCells2D[x, y];
            if (cell.occupyingTile != null)
            {
                if (ParticleSpawner.Instance != null)
                    ParticleSpawner.Instance.Spawn(cell.occupyingTile.ColorIndex, cell.occupyingTile);
                Destroy(cell.occupyingTile.gameObject);
            }
            cell.SetOccupied(null);
        }
    }
}
