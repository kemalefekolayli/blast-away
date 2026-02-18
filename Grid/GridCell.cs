using UnityEngine;

public class GridCell : MonoBehaviour
{
    public Vector2Int gridPosition;
    public Vector3 worldPosition => transform.position;
    public bool isOccupied = false;

    public void setOccupied(bool occupied)
    {
        isOccupied = occupied;
    }
   
   public void Initialize(Vector2Int gridPos, Vector3 worldPos)
   {
       this.gridPosition = gridPos;
       transform.position = worldPos;
       isOccupied = false;
   }
}