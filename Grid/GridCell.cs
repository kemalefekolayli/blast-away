using UnityEngine;

public class GridCell : MonoBehaviour
{
    public Vector2Int gridPosition;
    public Vector3 worldPosition => transform.position;
    public bool isOccupied = false;
    public Tile occupyingTile = null;

    public void SetOccupied(Tile tile)
    {
        occupyingTile = tile;
        isOccupied = tile != null;
    }

    public void Initialize(Vector2Int gridPos, Vector3 worldPos)
    {
        this.gridPosition = gridPos;
        transform.position = worldPos;
        isOccupied = false;
        occupyingTile = null;
    }

    public void SetGridPosition(Vector2Int newGridPos)
    {
        gridPosition = newGridPos;
    }
}
