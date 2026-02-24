using UnityEngine;
using TMPro;

public class ScoreText : MonoBehaviour
{
    public TMP_Text text;
    public ScoreKeeper scoreKeeper;
    void OnEnable()
    {
        GameEvents.OnScoreUpdate += UpdateText;
    }

    void OnDisable()
    {
        GameEvents.OnScoreUpdate -= UpdateText;
    }

    public void UpdateText()
    {
        text.text = scoreKeeper.currentScore.ToString();
    }
}