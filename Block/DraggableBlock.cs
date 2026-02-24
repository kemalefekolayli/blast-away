using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class DraggableBlock : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 startPosition;
    private int originalSiblingIndex;
    private Block block;
    public BlockShadow blockShadow;
    
    private Vector2 draggedTileOffset;

    [Header("Snap Ayarı")]
    public float snapThreshold = 80f;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        block = GetComponent<Block>();
        blockShadow = GetComponentInChildren<BlockShadow>();
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (block.IsPlaced()) return;

        blockShadow.CreateShadow((BlockShape)block.blockShapeIndex);

        startPosition = rectTransform.anchoredPosition;
        originalSiblingIndex = transform.GetSiblingIndex();
        transform.SetAsLastSibling(); // En öne çıkar

        // Mouse'un Canvas üzerindeki pozisyonunu bul
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 mouseCanvasPos
        );

        // Block'taki tile'lar arasından mouse'a en yakın olanı bul
        Tile closestTile = null;
        float closestDist = float.MaxValue;

        block.PickUp();

        foreach (Tile tile in block.tilesInShape)
        {
            RectTransform tileRect = tile.GetComponent<RectTransform>();

            // Tile'ın Canvas üzerindeki pozisyonunu bul
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.GetComponent<RectTransform>(),
                RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, tileRect.position),
                eventData.pressEventCamera,
                out Vector2 tileCanvasPos
            );

            float dist = Vector2.Distance(mouseCanvasPos, tileCanvasPos);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestTile = tile;
                // Bu tile'ın Block pivot'una göre local offset'i
                draggedTileOffset = tileRect.anchoredPosition;
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {   
        if (block.IsPlaced()) return;
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        // Mouse'un Canvas üzerindeki güncel pozisyonunu al
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 mouseCanvasPos
        );

        // Geçerli bir snap noktası var mı kontrol et
        if (CanWeSnapHere(mouseCanvasPos, eventData, out Vector2 validSnapPos))
            blockShadow.MoveTo(validSnapPos);
        else
            blockShadow.RemoveShadow();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (block.IsPlaced()) return;
        if (Grid.Instance == null)
        {
            rectTransform.anchoredPosition = startPosition;
            block.Drop();
            return;
        }

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 mouseCanvasPos
        );
        blockShadow.RemoveShadow();

        if (CanWeSnapHere(mouseCanvasPos, eventData, out Vector2 validSnapPos)){
            PlaceTheBlock(_lastValidAnchorGridPos, eventData);
        }
        else{
            ReturnToStartPos();
        }
    }


    private void ReturnToStartPos()
    {
        rectTransform.anchoredPosition = startPosition;
        transform.SetSiblingIndex(originalSiblingIndex);
        block.Drop();
    }

    // Sürüklenen tile'ın shape offset'ini döndürür (BlockShapeData içindeki offset)
    private Vector2Int GetDraggedTileShapeOffset()
    {
        Vector2Int[] offsets = BlockShapeData.Offsets[(BlockShape)block.blockShapeIndex];
        
        // Sürüklenen tile'ın hangi offset'e karşılık geldiğini bul
        Tile closestTile = null;
        float closestDist = float.MaxValue;
        int closestIdx = 0;

        for (int i = 0; i < block.tilesInShape.Count; i++)
        {
            RectTransform tileRect = block.tilesInShape[i].GetComponent<RectTransform>();
            float dist = Vector2.Distance(tileRect.anchoredPosition, draggedTileOffset);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestIdx = i;
            }
        }

        return offsets[closestIdx];
    }

    private void PlaceTheBlock(Vector2Int anchorGridPos, PointerEventData eventData)
    {
        Vector2Int[] offsets = BlockShapeData.Offsets[(BlockShape)block.blockShapeIndex];
        Grid.Instance.OccupyShape(anchorGridPos, offsets, block.tilesInShape);
        
        // Bloğu grid'in referans pozisyonuna taşı (tile'lar zaten OccupyShape'de yerleşti)
        block.SetPlaced(true);
        GameEvents.BlockPlaced();
    }

    public bool CanWeSnapHere(Vector2 mouseCanvasPos, PointerEventData eventData, out Vector2 validSnapPos)
    {
        validSnapPos = Vector2.zero;
        
        // Mouse'a en yakın grid hücresini bul
        Vector2Int? cellGridPos = Grid.Instance.GetClosestCellGridPos(mouseCanvasPos, snapThreshold);
        if (!cellGridPos.HasValue) return false;

        // Sürüklenen tile'ın shape offset'ini çıkar → bloğun anchor grid pozisyonu
        Vector2Int dragOffset = GetDraggedTileShapeOffset();
        Vector2Int anchorGridPos = cellGridPos.Value - dragOffset;

        // Şekil bu pozisyona sığıyor mu? (grid koordinat bazlı — kesin sonuç)
        Vector2Int[] offsets = BlockShapeData.Offsets[(BlockShape)block.blockShapeIndex];
        if (!Grid.Instance.CanOccupyShape(anchorGridPos, offsets))
            return false;

        // Sığıyorsa, shadow için canvas pozisyonunu hesapla
        // Anchor cell'in canvas pozisyonundan draggedTileOffset'i çıkar
        Vector2 anchorCellCanvasPos = Grid.Instance.GetCellCanvasPosition(anchorGridPos);
        validSnapPos = anchorCellCanvasPos;
        
        // anchorGridPos'u geçici olarak sakla
        _lastValidAnchorGridPos = anchorGridPos;

        return true;
    }

    private Vector2Int _lastValidAnchorGridPos;
}
