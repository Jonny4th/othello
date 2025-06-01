using Core;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Bot
{
    private GameRules m_GameRule;

    public event Action<Coordinates> OnBotMoveMade;

    public void SetGameRules(GameRules ruleBook)
    {
        m_GameRule = ruleBook;
    }

    public void MakeDecision(BoardState boardState, Occupancy botColor)
    {
        var legalMoves = m_GameRule.FindLegalMoves(boardState, botColor);

        if (legalMoves.Length == 0)
        {
            Debug.Log("No legal moves available for the bot.");
            OnBotMoveMade?.Invoke(new Coordinates(-1,-1));
        }

        // Randomly select a move from the legal moves
        var randomIndex = Random.Range(0, legalMoves.Length);
        var selectedMove = legalMoves[randomIndex];

        // Place the disc at the selected move
        OnBotMoveMade?.Invoke(new Coordinates(selectedMove.Item1, selectedMove.Item2));
        Debug.Log($"Bot placed a disc at {selectedMove}");
    }
}
