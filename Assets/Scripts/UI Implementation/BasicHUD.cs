using Core;
using TMPro;
using UnityEngine;

public class BasicHUD : BaseHUD
{
    [SerializeField]
    private TMP_Text m_ScoreDisplay;

    [SerializeField]
    private TMP_Text m_TurnDisplay;

    private const string m_ScoreFormat = "Score :\nBlack {0}\nWhite {1}";

    public override void SetPlayerTurn(Occupancy currentPlayer)
    {
        m_TurnDisplay.text = currentPlayer.ToString();

        switch(currentPlayer)
        {
            case Occupancy.Black:
                m_TurnDisplay.color = Color.black;
                break;
            case Occupancy.White:
                m_TurnDisplay.color = Color.white;
                break;
        }
    }

    public override void SetScore(int black, int white)
    {
        m_ScoreDisplay.text = string.Format(m_ScoreFormat, black, white);
    }
}
