using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{

    [Header("Visual Components")]
    [SerializeField] private Image tileImage;

    public Vector2Int gridPosition;
    private int colorIndex; // refers to the index of the color in the TileColor enum

    public int ColorIndex => colorIndex;
    public Vector2Int GridPosition => gridPosition;

    #region Initialization
    
    private void Awake()
    {
        if (tileImage == null)
            tileImage = GetComponentInChildren<Image>();
    }
    
    public void Initialize(int colorIndex, Vector2Int gridPos, Sprite sprite)
    {   
        this.colorIndex = colorIndex;
        this.gridPosition = gridPos;
        
        if (sprite != null)
            tileImage.sprite = sprite;
        
        transform.localScale = Vector3.one;
        
        gameObject.SetActive(true);
    }
    
    #endregion
    
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}