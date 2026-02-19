using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

// Sahneye bir kez koy, Inspector'dan ParticleData ata
// Tile yok edildiğinde: ParticleSpawner.Instance.Spawn(colorIndex, tile)
public class ParticleSpawner : MonoBehaviour
{
    public static ParticleSpawner Instance { get; private set; }

    [System.Serializable]
    private struct ParticleEntry
    {
        public ParticleData data;
        public ParticlePool pool;
    }

    [Header("Her index bir TileData rengiyle eşleşmeli")]
    [SerializeField] private ParticleEntry[] entries;

    [Header("Particle'ların spawn edileceği Canvas container")]
    [SerializeField] private RectTransform particleContainer;

    private Canvas canvas;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        canvas = GetComponentInParent<Canvas>();
        if (canvas == null) canvas = FindFirstObjectByType<Canvas>();
    }

    // Tile'ın dünya pozisyonundan canvas pozisyonuna çevirip particle spawn eder
    public void Spawn(int colorIndex, Tile tile)
    {
        if (colorIndex < 0 || colorIndex >= entries.Length) return;

        ParticleEntry entry = entries[colorIndex];
        if (entry.data == null || entry.pool == null) return;

        // Tile'ın dünya pozisyonunu canvas lokal koordinatına çevir
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            particleContainer,
            RectTransformUtility.WorldToScreenPoint(null, tile.transform.position),
            null,
            out Vector2 canvasPos
        );

        for (int i = 0; i < entry.data.count; i++)
        {
            Particle p = entry.pool.Get();
            p.transform.SetParent(particleContainer, false); // Canvas içinde tut
            p.Play(entry.data, canvasPos, (finished) => entry.pool.Return(finished));
        }
    }
}
