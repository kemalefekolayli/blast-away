using UnityEngine;

[CreateAssetMenu(fileName = "ParticleData", menuName = "Blast Game/Particle Data")]
public class ParticleData : ScriptableObject
{
    [Header("Görsel")]
    public Sprite sprite;
    public Color color = Color.white;

    [Header("Spawn")]
    [Range(1, 20)] public int count = 6;          // Kaç particle çıkacak

    [Header("Hareket")]
    public float minSpeed = 150f;
    public float maxSpeed = 300f;

    [Header("Ömür")]
    public float minLifetime = 0.3f;
    public float maxLifetime = 0.6f;

    [Header("Boyut")]
    public float startSize = 18f;
    public float endSize   = 0f;
}
