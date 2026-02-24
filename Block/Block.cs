using UnityEngine;
using System.Collections.Generic;

public class Block : MonoBehaviour
{
    public int blockShapeIndex;
    public int blockColorIndex;
    public CanvasGroup canvasGroup;
    public bool hasBeenPlaced = false;

    [Header("Scale Ayarları")]
    public float holderScale = 0.6f;  // Holder'da dururken
    public float dragScale   = 1.0f;  // Drag sırasında

    public List<Tile> tilesInShape = new List<Tile>();

    public void Initialize(int shapeIndex, int colorIndex, List<Tile> tiles)
    {
        this.blockShapeIndex = shapeIndex;
        this.blockColorIndex = colorIndex;
        this.tilesInShape = tiles;

        canvasGroup.alpha = 1f;
        // Spawn'da holder küçük boyutuyla başla
        transform.localScale = Vector3.one * holderScale;
        gameObject.SetActive(true);
    }

    public void SetPlaced(bool placed)
    {
        hasBeenPlaced = placed;
    }

    public bool IsPlaced() => hasBeenPlaced;

    public void OnEnable()
    {
        GameEvents.OnBlockPickedUp += OnBlockPickedUp;
        GameEvents.OnBlockDropped  += OnBlockDropped;
    }
    public void OnDisable()
    {
        GameEvents.OnBlockPickedUp -= OnBlockPickedUp;
        GameEvents.OnBlockDropped  -= OnBlockDropped;
    }
    private void OnBlockPickedUp() { }
    private void OnBlockDropped()  { }
    public void PickUp()
    {
        transform.localScale = Vector3.one * dragScale;
    }
    public void Drop()
    {
        if (!hasBeenPlaced)
            transform.localScale = Vector3.one * holderScale;
        // placed ise dragScale'de kalır
    }

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }
}
