using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverPanel : MonoBehaviour
{
    [Header("Referanslar")]
    public TextMeshProUGUI scoreText;

    private CanvasGroup canvasGroup;
    private Canvas overrideCanvas;

    void OnEnable()
    {
        GameEvents.OnGameOver += ShowPanel;
    }

    void OnDisable()
    {
        GameEvents.OnGameOver -= ShowPanel;
    }

    void Start()
    {
        // CanvasGroup yoksa ekle
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // Kendi Canvas'ını ekle ve sorting order'ı en üste çek
        overrideCanvas = GetComponent<Canvas>();
        if (overrideCanvas == null)
            overrideCanvas = gameObject.AddComponent<Canvas>();
        overrideCanvas.overrideSorting = true;
        overrideCanvas.sortingOrder = 100;

        // Butonların tıklanabilmesi için GraphicRaycaster gerekli
        if (GetComponent<UnityEngine.UI.GraphicRaycaster>() == null)
            gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        // Başlangıçta gizle
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    void ShowPanel()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        ScoreKeeper keeper = FindFirstObjectByType<ScoreKeeper>();
        if (keeper != null && scoreText != null)
            scoreText.text = "Skor: " + keeper.currentScore;
    }

    public void OnRestartClicked()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
}
