using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableBlock : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 startPosition;
    private Block block;
    private Vector2 draggedTileOffset;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        block = GetComponent<Block>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (block.IsPlaced()!) return;
        
        startPosition = rectTransform.anchoredPosition;

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
        if (block.IsPlaced()!) return;
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (block.IsPlaced()!) return;
        if (Grid.Instance == null)
        {
            rectTransform.anchoredPosition = startPosition;
            return;
        }

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 mouseCanvasPos
        );

        Vector2? snapPos = Grid.Instance.GetClosestCellPosition(mouseCanvasPos);

        if (snapPos.HasValue)
        {
            // Block'u öyle konumlandır ki sürüklenen tile o cell'e denk gelsin
            rectTransform.anchoredPosition = snapPos.Value - draggedTileOffset;
            block.SetPlaced(true);
        }
        else
        {
            // Grid dışına bırakıldıysa başlangıç pozisyonuna dön
            rectTransform.anchoredPosition = startPosition;
        }
    }
}
