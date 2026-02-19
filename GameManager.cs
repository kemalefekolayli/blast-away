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
        for (int i = 0; i < activeBlockCount; i++)
        {
            BlockShape shape = (BlockShape)shapes[i];
            blockSpawner.SpawnBlock(shape,  blockHolder.anchoredPosition + new Vector2((i-1) * offset, 0) );
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

    public void OnNoBlocksLeft()
    {
        SpawnBlocks();
    }

    public void RandomizeBlockShapes(int count)
    {   
        shapes.Clear();
        for (int i = 0; i < count; i++)
        {
            int randomShape = Random.Range(0, System.Enum.GetValues(typeof(BlockShape)).Length);
            shapes.Add(randomShape);
        }
    }
}