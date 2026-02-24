using UnityEngine;
using System.Collections.Generic;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public BlockSpawner blockSpawner;
    public CellSpawner cellSpawner;
    public int activeBlockCount;
    private List<int> shapes = new List<int>();
    public RectTransform blockHolder;
    public float offset; 

    private void OnEnable()
    {
        GameEvents.OnBlockPlaced += OnBlockPlaced;
        GameEvents.NoBlocksLeft += OnNoBlocksLeft;
    }
    private void OnDisable()
    {
        GameEvents.OnBlockPlaced -= OnBlockPlaced;
        GameEvents.NoBlocksLeft -= OnNoBlocksLeft;
    }
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

    private void SpawnBlocks()
    {
        if(activeBlockCount > 0) return;
        activeBlockCount = 3; 
        RandomizeBlockShapes(activeBlockCount);

        // Eğer hiçbir şekil tahtaya sığmıyorsa — Game Over
        bool anyFits = false;
        foreach (int s in shapes)
        {
            if (Grid.Instance.CanShapeFitAnywhere((BlockShape)s))
            {
                anyFits = true;
                break;
            }
        }

        if (!anyFits)
        {
            GameEvents.OnGameOver?.Invoke();
            return;
        }

        for (int i = 0; i < activeBlockCount; i++)
        {
            BlockShape shape = (BlockShape)shapes[i];
            blockSpawner.SpawnBlock(shape, blockHolder.anchoredPosition + new Vector2((i-1) * offset, 0));
        }
    }

    void Start()
    {   
        cellSpawner.FillGrid();
        SpawnBlocks();
    }

    public void OnBlockPlaced()
    {
        activeBlockCount--;

        if (activeBlockCount <= 0)
        {
            GameEvents.TriggerNoBlocksLeft();
            return;
        }

        // Kalan yerleştirilmemiş bloklardan en az biri tahtaya sığıyor mu?
        Block[] allBlocks = FindObjectsByType<Block>(FindObjectsSortMode.None);
        bool anyRemainingFits = false;
        foreach (Block b in allBlocks)
        {
            if (!b.IsPlaced())
            {
                BlockShape shape = (BlockShape)b.blockShapeIndex;
                if (Grid.Instance.CanShapeFitAnywhere(shape))
                {
                    anyRemainingFits = true;
                    break;
                }
            }
        }

        if (!anyRemainingFits)
        {
            GameEvents.OnGameOver?.Invoke();
        }
    }

    public void OnNoBlocksLeft()
    {
        SpawnBlocks();
    }

    public void RandomizeBlockShapes(int count)
    {
        shapes.Clear();

        // Her şeklin ağırlığı = tile sayısı (büyük parçalara daha yüksek şans)
        var allShapes = (BlockShape[])System.Enum.GetValues(typeof(BlockShape));
        List<BlockShape> weightedPool = new List<BlockShape>();

        foreach (BlockShape shape in allShapes)
        {
            int tileCount = BlockShapeData.Offsets[shape].Length;
            for (int w = 0; w < tileCount; w++)
                weightedPool.Add(shape);
        }

        const int maxAttempts = 40;

        for (int i = 0; i < count; i++)
        {
            BlockShape picked = BlockShape.Single; // fallback
            bool found = false;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                BlockShape candidate = weightedPool[Random.Range(0, weightedPool.Count)];
                if (Grid.Instance.CanShapeFitAnywhere(candidate))
                {
                    picked = candidate;
                    found = true;
                    break;
                }
            }

            if (!found)
                Debug.LogWarning("Tahta çok dolu — fallback olarak Single veriliyor.");

            shapes.Add((int)picked);
        }
    }
}