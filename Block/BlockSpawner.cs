using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class BlockSpawner : MonoBehaviour
{
    public Tile TilePrefab; 
    public Block blockPrefab;
    public TileData[] tileDatas; // Her eleman bir renk — Inspector'dan istediğin kadar ekle
    public Transform blockContainer;
    public float tileSpacing = 0f; // Tile'lar arası boşluk (piksel)

    // Tile'ı verilen parent'ın child'ı olarak spawn eder
    public Tile SpawnTile(int colorIndex, Vector2Int gridPos, Vector2 anchoredPos, Transform parent)
    {
        Sprite sprite = tileDatas[colorIndex].sprite;
        Tile newTile = Instantiate(TilePrefab, parent);
        newTile.GetComponent<RectTransform>().anchoredPosition = anchoredPos;
        newTile.Initialize(colorIndex, gridPos, sprite);
        return newTile;
    }

    // Shape'e göre Block ve tile'larını oluşturur
    public Block SpawnBlock(BlockShape shape, int colorIndex, Vector2 blockCenter)
    {
        // Önce Block'u spawn et
        Block newBlock = Instantiate(blockPrefab, blockContainer);
        newBlock.GetComponent<RectTransform>().anchoredPosition = blockCenter;
        int blockColor = GetRandomColorIndex();
        // Tile boyutunu prefab'ın RectTransform'undan oku
        float tileSize = TilePrefab.GetComponent<RectTransform>().sizeDelta.x;
        float step = tileSize + tileSpacing;

        // Sonra tile'ları Block'un child'ı olarak spawn et
        Vector2Int[] offsets = BlockShapeData.Offsets[shape];
        List<Tile> tiles = new List<Tile>();

        for (int i = 0; i < offsets.Length; i++)
        {
            Vector2 localPos = new Vector2(offsets[i].x * step, offsets[i].y * step);
            Tile tile = SpawnTile(blockColor, offsets[i], localPos, newBlock.transform);
            tiles.Add(tile);
        }

        newBlock.Initialize((int)shape, blockColor, tiles);
        return newBlock;
    }

    public int GetRandomColorIndex()
    {
        return Random.Range(0, tileDatas.Length);
    }
}
