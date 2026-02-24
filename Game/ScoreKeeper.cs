using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    public int consecutiveScoringTurns; // Üst üste kaç turdur skor alındı
    public int multiplier = 1;
    public int currentScore;
    private bool scoredThisTurn;

    void Start()
    {
        currentScore = 0;
        multiplier = 1;
    }

    void OnEnable()
    {
        GameEvents.OnCellCleared += OnCellCleared;
        GameEvents.NoBlocksLeft  += OnTurnEnd;
    }

    void OnDisable()
    {
        GameEvents.OnCellCleared -= OnCellCleared;
        GameEvents.NoBlocksLeft  -= OnTurnEnd;
    }

    void OnCellCleared()
    {
        scoredThisTurn = true;
        currentScore += 10 * multiplier;
        GameEvents.ScoreUpdate();
    }

    void OnTurnEnd()
    {
        if (scoredThisTurn)
        {
            consecutiveScoringTurns++;
        }
        else
        {
            consecutiveScoringTurns = 0;
        }

        multiplier = Mathf.Max(1, consecutiveScoringTurns);
        scoredThisTurn = false;
    }
}