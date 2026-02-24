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
    private List<Vector2> tileCanvasPositions = new List<Vector2>();

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
            PlaceTheBlock(validSnapPos, eventData);
        }
        else{
            // Dolu cell var veya çok uzak — geri dön ve eski sıraya dön
            ReturnToStartPos();
        }
    }


    private void ReturnToStartPos()
    {
        rectTransform.anchoredPosition = startPosition;
        transform.SetSiblingIndex(originalSiblingIndex);
        block.Drop();
    }
    private void CalculateTileCanvasPosition(Vector2 blockAnchor)
    {
        tileCanvasPositions.Clear();

        foreach (Tile tile in block.tilesInShape)
        {
            RectTransform tileRect = tile.GetComponent<RectTransform>();
            // Tile'ın canvas'taki beklenen konumu = bloğun hedef anchor'u + tile'ın blok içindeki yerel ofseti
            Vector2 expectedPos = blockAnchor + tileRect.anchoredPosition;
            tileCanvasPositions.Add(expectedPos);
        }
    }

    private void PlaceTheBlock(Vector2 blockAnchor, PointerEventData eventData)
    {
        rectTransform.anchoredPosition = blockAnchor;

        CalculateTileCanvasPosition(blockAnchor);
        
        bool success = Grid.Instance.OccupyCells(tileCanvasPositions, block.tilesInShape, snapThreshold);
        block.SetPlaced(true);
        GameEvents.BlockPlaced();
        
    }

    public bool CanWeSnapHere(Vector2 mouseCanvasPos, PointerEventData eventData, out Vector2 validSnapPos)
    {
        validSnapPos = Vector2.zero;
        Vector2? snapPos = Grid.Instance.GetClosestCellPosition(mouseCanvasPos);

        if (snapPos.HasValue && Vector2.Distance(mouseCanvasPos, snapPos.Value) <= snapThreshold)
        {
            Vector2 blockAnchor = snapPos.Value - draggedTileOffset;
            
            CalculateTileCanvasPosition(blockAnchor);

            bool success = Grid.Instance.CanOccupyCells(tileCanvasPositions, block.tilesInShape, snapThreshold);
            if (success)
            {
                validSnapPos = blockAnchor;
                return true;
            }
        }
        
        return false;
    }
}
