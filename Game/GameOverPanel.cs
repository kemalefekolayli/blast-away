using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverPanel : MonoBehaviour
{
    [Header("Referanslar")]
    public GameObject panelRoot;   // Panelin kök objesi
    public TextMeshProUGUI scoreText;

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
        panelRoot.SetActive(false);
    }

    void ShowPanel()
    {
        panelRoot.SetActive(true);

        // ScoreKeeper sahnesinde varsa skoru çek
        ScoreKeeper keeper = FindFirstObjectByType<ScoreKeeper>();
        if (keeper != null && scoreText != null)
            scoreText.text = "Skor: " + keeper.currentScore;
    }

    // Restart butonu bu metodu çağırır
    public void OnRestartClicked()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
}
