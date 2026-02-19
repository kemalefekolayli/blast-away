using UnityEngine;

public class CellSpawner : MonoBehaviour
{
    public GameObject cellPrefab;
    public Grid gridObject;
    [SerializeField] public float cellOffset_x;
    [SerializeField] public float cellOffset_y;

    public void FillGrid()
    {
        for (int x = 0; x < gridObject.width; x++)
        {
            for (int y = 0; y < gridObject.height; y++)
            {
                Vector2Int gridPos = new Vector2Int(x, y);
                Vector3 originalPos = gridObject.transform.position;
                Vector3 worldPos = new Vector3(originalPos.x + x * cellOffset_x, originalPos.y + y * cellOffset_y, originalPos.z);
                GameObject newCell = Instantiate(cellPrefab, worldPos, Quaternion.identity, gridObject.transform);

                Vector2Int cellGridPos = new Vector2Int(x, y);
                newCell.GetComponent<GridCell>().SetGridPosition(cellGridPos);
                gridObject.AddCell(newCell.GetComponent<GridCell>());
            }
        }

        // Cell'ler oluşturuldu, pozisyonları cache'le
        gridObject.BakePositions();
    }
}
