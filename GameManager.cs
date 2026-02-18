using UnityEngine;
using System.Collections.Generic;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public BlockSpawner blockSpawner;
    public CellSpawner cellSpawner;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Test: ekran ortasında 3'lü yatay kırmızı block spawn et
        blockSpawner.SpawnBlock(BlockShape.LongHorizontalLine_3, 0, Vector2.zero);

        cellSpawner.FillGrid();
    }
}