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
        GameEvents.OnExplosionsResolved += CheckForDeadlock;
    }
    private void OnDisable()
    {
        GameEvents.OnBlockPlaced -= OnBlockPlaced;
        GameEvents.NoBlocksLeft -= OnNoBlocksLeft;
        GameEvents.OnExplosionsResolved -= CheckForDeadlock;
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
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
        }
    }

    // Patlamalar bittikten sonra kalan bloklar sığıyor mu kontrol et
    void CheckForDeadlock()
    {
        if (activeBlockCount <= 0) return; // Tur zaten bitti, SpawnBlocks halleder

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

        var allShapes = (BlockShape[])System.Enum.GetValues(typeof(BlockShape));
        float occupancyRate = Grid.Instance.GetOccupancyRate();

        const int maxAttempts = 50;

        for (int i = 0; i < count; i++)
        {
            // Sığabilen şekilleri ve ağırlıklarını hesapla
            List<BlockShape> candidates = new List<BlockShape>();
            List<float> weights = new List<float>();
            float totalWeight = 0f;

            foreach (BlockShape shape in allShapes)
            {
                if (!Grid.Instance.CanShapeFitAnywhere(shape)) continue;

                int tileCount = BlockShapeData.Offsets[shape].Length;
                float weight;

                if (occupancyRate < 0.4f)
                {
                    // Tahta boşken büyük parçalara ağırlık ver
                    weight = tileCount * tileCount;
                }
                else if (occupancyRate < 0.7f)
                {
                    // Orta dolulukta dengeli
                    weight = tileCount;
                }
                else
                {
                    // Tahta doluyken küçük parçalara ağırlık ver
                    weight = 1f / tileCount;
                }

                // Şekil bazlı ayarlar
                string name = shape.ToString();
                if (name.StartsWith("Lshape"))
                    weight *= 0.3f; // L şekilleri daha az gelsin
                else if (shape == BlockShape.Square_2x2 || 
                         shape == BlockShape.RectangleVertical_2x3 || 
                         shape == BlockShape.RectangleHorizontal_2x3)
                    weight *= 2f; // Kare ve dikdörtgenler daha çok gelsin

                candidates.Add(shape);
                weights.Add(weight);
                totalWeight += weight;
            }

            if (candidates.Count == 0)
            {
                shapes.Add((int)BlockShape.Single);
                continue;
            }

            // Ağırlıklı rastgele seçim
            float roll = Random.Range(0f, totalWeight);
            float cumulative = 0f;
            BlockShape picked = candidates[0];

            for (int j = 0; j < candidates.Count; j++)
            {
                cumulative += weights[j];
                if (roll <= cumulative)
                {
                    picked = candidates[j];
                    break;
                }
            }

            shapes.Add((int)picked);
        }
    }
}