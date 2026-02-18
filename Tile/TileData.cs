using UnityEngine;

[CreateAssetMenu(fileName = "TileData", menuName = "Blast Game/Tile Data")]
public class TileData : ScriptableObject
{
    [Header("Color Info")]
    public int colorIndex;
    public string colorName;
    
    [Header("Sprites")]
    public Sprite sprite;
}