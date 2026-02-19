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
    private Vector2 draggedTileOffset;

    [Header("Snap Ayarı")]
    public float snapThreshold = 80f;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        block = GetComponent<Block>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (block.IsPlaced()) return;
        
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

        Vector2? snapPos = Grid.Instance.GetClosestCellPosition(mouseCanvasPos);

        // Snap eşiği kontrolü — çok uzaksa snap yapma
        if (snapPos.HasValue && Vector2.Distance(mouseCanvasPos, snapPos.Value) > snapThreshold)
            snapPos = null;

        if (snapPos.HasValue)
        {
            // Block'u snap pozisyonuna taşı
            Vector2 blockAnchor = snapPos.Value - draggedTileOffset;
            rectTransform.anchoredPosition = blockAnchor;

            // Tüm tile'ların canvas pozisyonlarını hesapla
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            List<Vector2> tileCanvasPositions = new List<Vector2>();

            foreach (Tile tile in block.tilesInShape)
            {
                RectTransform tileRect = tile.GetComponent<RectTransform>();
                // Tile'ın yeni dünya pozisyonunu canvas koordinatına çevir
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvasRect,
                    RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, tileRect.position),
                    eventData.pressEventCamera,
                    out Vector2 tileCanvasPos
                );
                tileCanvasPositions.Add(tileCanvasPos);
            }

            // Tüm cell'lerin boş olup olmadığını kontrol et ve dolu işaretle
            bool success = Grid.Instance.TryOccupyCells(tileCanvasPositions, block.tilesInShape, snapThreshold);

            if (success)
            {
                block.SetPlaced(true);
                GameEvents.BlockPlaced();
            }
            else
            {
                // Dolu cell var — geri dön ve eski sıraya dön
                rectTransform.anchoredPosition = startPosition;
                transform.SetSiblingIndex(originalSiblingIndex);
            }
        }
        else
        {
            // Grid dışı — geri dön ve eski sıraya dön
            rectTransform.anchoredPosition = startPosition;
            transform.SetSiblingIndex(originalSiblingIndex);
        }

        block.Drop();
    }
}
