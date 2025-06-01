using Core;
using TMPro;
using UnityEngine;

public class BasicHUD : BaseHUD
{
    [SerializeField]
    private TMP_Text m_ScoreDisplay;

    [SerializeField]
    private TMP_Text m_TurnDisplay;

    [SerializeField]
    private Color m_WhiteTurnTextColor = Color.white;

    [SerializeField]
    private Color m_BlackTurnTextColor = Color.black;

    private string m_ScoreFormat = "Score :\n{0} {1}\n{2} {3}";

    private string m_WhitePlayerName;
    private string m_BlackPlayerName;

    public override void SetPlayerTurn(Occupancy currentPlayer)
    {
        var name = currentPlayer switch
        {
            Occupancy.Black => m_BlackPlayerName,
            Occupancy.White => m_WhitePlayerName,
            _ => "Unknown Player"
        };

        m_TurnDisplay.text = name;

        switch(currentPlayer)
        {
            case Occupancy.Black:
                m_TurnDisplay.color = m_BlackTurnTextColor;
                break;
            case Occupancy.White:
                m_TurnDisplay.color = m_WhiteTurnTextColor;
                break;
        }
    }

    public override void SetScore(int blackScore, int whiteScore)
    {
        m_ScoreDisplay.text = string.Format(m_ScoreFormat, m_BlackPlayerName, blackScore, m_WhitePlayerName, whiteScore);
    }

    public override void SetPlayerName(string black, string white)
    {
        m_BlackPlayerName = black;
        m_WhitePlayerName = white;
    }
}
