using UnityEngine;
using System.Collections.Generic;
public class Block : MonoBehaviour
{
    public int blockShapeIndex;
    public int blockColorIndex;
    public CanvasGroup canvasGroup;
    public bool hasBeenPlaced = false ;


    public List<Tile> tilesInShape = new List<Tile>();

    public void Initialize(int shapeIndex, int colorIndex, List<Tile> tiles)
    {
        this.blockShapeIndex = shapeIndex;
        this.blockColorIndex = colorIndex;
        this.tilesInShape = tiles;

        canvasGroup.alpha = 1f;
        transform.localScale = Vector3.one;
        
        gameObject.SetActive(true);
    }

    public void SetPlaced(bool placed)
    {
        hasBeenPlaced = placed;
    }
    public bool IsPlaced() => hasBeenPlaced;
}