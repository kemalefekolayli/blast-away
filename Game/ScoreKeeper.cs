using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    public int consecutiveScoringTurns;
    public int multiplier = 1;
    public int currentScore;
    private bool scoredThisTurn;

    void Start()
    {
        currentScore = 0;
        multiplier = 1;
        consecutiveScoringTurns = 0;
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
        // Bu turdaki ilk blast mı?
        bool firstBlastThisTurn = !scoredThisTurn;
        scoredThisTurn = true;

        currentScore += 10 * multiplier;
        GameEvents.ScoreUpdate();

        // Bu turdaki ilk blast anında combo popup'ı göster
        // Önceki turlardan gelen streak + bu tur = toplam ardışık tur
        if (firstBlastThisTurn)
        {
            int currentStreak = consecutiveScoringTurns + 1; // +1 = bu tur dahil
            if (currentStreak > 1)
                GameEvents.OnComboUpdate?.Invoke(currentStreak);
        }
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