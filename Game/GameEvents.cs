using UnityEngine;
using System;

public static class GameEvents
{
    public static Action OnBlockPickedUp;
    public static Action OnBlockDropped;
    public static Action OnBlockPlaced;
    public static Action OnBlockRemoved;
    public static Action OnCellFilled;
    public static Action OnCellCleared;
    public static Action NoBlocksLeft;
    public static Action OnScoreUpdate;
    public static Action OnGameOver;
    public static Action<int> OnComboUpdate; // int = multiplier değeri
    public static Action OnExplosionsResolved; // Patlamalar bittikten sonra

    public static void ScoreUpdate()
    {
        OnScoreUpdate?.Invoke();
    }

    public static void TriggerBlockPickedUp()
    {
        OnBlockPickedUp?.Invoke();
    }
    public static void TriggerBlockDropped()
    {
        OnBlockDropped?.Invoke();
    }
    public static void TriggerNoBlocksLeft()
    {
        NoBlocksLeft?.Invoke();
    }
    public static void BlockPlaced()
    {
        OnBlockPlaced?.Invoke();
    }
    public static void BlockRemoved()
    {
        OnBlockRemoved?.Invoke();
    }

    public static void CellFilled()
    {
        OnCellFilled?.Invoke();
    }

    public static void CellCleared()
    {
        OnCellCleared?.Invoke();
    }
}