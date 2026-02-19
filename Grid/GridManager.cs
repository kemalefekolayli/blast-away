using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }
    [SerializeField] private Grid grid;
    public void OnEnable()
    {
        GameEvents.OnBlockPlaced += CheckForExplosion;
        GameEvents.OnBlockRemoved += OnBlockRemoved;
    }

    public void OnDisable()
    {
        GameEvents.OnBlockPlaced -= CheckForExplosion;
        GameEvents.OnBlockRemoved -= OnBlockRemoved;
    }

    public void CheckForExplosion()
    {
        CheckColums();
        CheckRows();
    }

    public void CheckRows()
    {
        for (int y = 0; y < grid.height; y++)
        {
            int count = 0;
            for (int x = 0; x < grid.width; x++)
            {
                if (grid.gridCells2D[x, y].isOccupied)
                    count++;
            }
            if (count == grid.width)
            {
                Debug.Log("Row " + y + " exploded!");
                grid.ClearRow(y);
                GameEvents.CellCleared();
            }
        }
    }

    public void CheckColums()
    {
        for (int x = 0; x < grid.width; x++)
        {
            int count = 0;
            for (int y = 0; y < grid.height; y++)
            {
                if (grid.gridCells2D[x, y].isOccupied)
                    count++;
            }
            if (count == grid.height)
            {
                Debug.Log("Column " + x + " exploded!");
                grid.ClearColumn(x);
                GameEvents.CellCleared();
            }
        }
    }

    public void OnBlockRemoved()
    {
        // Her block kaldırıldığında patlama kontrolü yap
        CheckForExplosion();
    }
}