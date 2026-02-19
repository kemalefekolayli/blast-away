using UnityEngine;
using UnityEngine.UI;

public class Particle : MonoBehaviour
{
    private Image image;
    private RectTransform rectTransform;

    private Vector2 direction;
    private float speed;
    private float lifetime;
    private float elapsed;
    private float startSize;
    private float endSize;
    private Color startColor;

    private System.Action<Particle> onFinished; // Pool'a geri dönmek için callback

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    public void Play(ParticleData data, Vector2 canvasPosition, System.Action<Particle> finishedCallback)
    {
        onFinished = finishedCallback;

        rectTransform.anchoredPosition = canvasPosition;

        // Rastgele yön
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        speed     = Random.Range(data.minSpeed, data.maxSpeed);
        lifetime  = Random.Range(data.minLifetime, data.maxLifetime);
        elapsed   = 0f;
        startSize = data.startSize;
        endSize   = data.endSize;
        startColor = data.color;

        image.sprite = data.sprite;
        image.color  = startColor;
        rectTransform.sizeDelta = Vector2.one * startSize;

        gameObject.SetActive(true);
    }

    private void Update()
    {
        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / lifetime);

        // Hareket
        rectTransform.anchoredPosition += direction * speed * Time.deltaTime;

        // Boyut küçülme
        float size = Mathf.Lerp(startSize, endSize, t);
        rectTransform.sizeDelta = Vector2.one * size;

        // Fade
        image.color = new Color(startColor.r, startColor.g, startColor.b, 1f - t);

        if (elapsed >= lifetime)
        {
            gameObject.SetActive(false);
            onFinished?.Invoke(this);
        }
    }
}
