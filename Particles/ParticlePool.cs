using UnityEngine;
using System.Collections.Generic;

// ParticleSpawner'ın altına attach edilir — her colorIndex için ayrı pool yönetir
public class ParticlePool : MonoBehaviour
{
    [SerializeField] private Particle particlePrefab;
    [SerializeField] private int initialPoolSize = 20;

    private Queue<Particle> pool = new Queue<Particle>();

    private void Awake()
    {
        // Başlangıçta pool'u doldur
        for (int i = 0; i < initialPoolSize; i++)
            CreateParticle();
    }

    private Particle CreateParticle()
    {
        Particle p = Instantiate(particlePrefab, transform);
        p.gameObject.SetActive(false);
        pool.Enqueue(p);
        return p;
    }

    public Particle Get()
    {
        // Pool boşsa yeni oluştur
        if (pool.Count == 0)
            CreateParticle();

        return pool.Dequeue();
    }

    public void Return(Particle p)
    {
        p.gameObject.SetActive(false);
        p.transform.SetParent(transform);
        pool.Enqueue(p);
    }
}
