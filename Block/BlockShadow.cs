using UnityEngine;

public class BlockShadow : MonoBehaviour
{
    // bir noktada buraya pooling eklicez
    
    public Block shadowBlock;
    public bool isActive = true;
    public BlockSpawner blockSpawner;
    private int idx;
    
    void OnEnable()
    {
        GameEvents.NoBlocksLeft += ClearBatch ;
    }

    void OnDisable()
    {
        GameEvents.NoBlocksLeft -= ClearBatch;
    }
    
    void ClearBatch()
    {
        
        isActive = false;
        if (shadowBlock != null)
            Destroy(shadowBlock.gameObject);
        shadowBlock = null;
        idx = 0;
    }
    
    void Start()
    {
        if(blockSpawner == null)
        {
            blockSpawner = FindFirstObjectByType<BlockSpawner>();
        }
        idx = 0;
    }

    public void CreateShadow(BlockShape blockShape)
    {   
        if(idx == 0)
        {
            shadowBlock = blockSpawner.SpawnBlock(blockShape, Vector2.zero);
            shadowBlock.PickUp();
            shadowBlock.canvasGroup.alpha = 0.5f;
            idx++;
        }
        else
        {
            shadowBlock.gameObject.SetActive(true);
        }
    }

    public void RemoveShadow()
    {
        if (shadowBlock != null)
            shadowBlock.gameObject.SetActive(false);
    }

    public void MoveTo(Vector2 anchoredPos)
    {
        if (shadowBlock == null) return;
        shadowBlock.gameObject.SetActive(true);
        shadowBlock.GetComponent<RectTransform>().anchoredPosition = anchoredPos;
    }

    
}