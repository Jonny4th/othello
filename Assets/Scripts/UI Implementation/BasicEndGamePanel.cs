using Main.UIs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BasicEndGamePanel : BaseEndGameDisplay
{
    [SerializeField]
    private TMP_Text m_WinnerDisplay;

    [SerializeField]
    private TMP_Text m_ScoreDisplay;

    [SerializeField]
    private Button m_RestartButton;

    private void OnEnable()
    {
        m_RestartButton.onClick.AddListener(() => OnRestartButtonPressed?.Invoke());
    }

    private void OnDisable()
    {
        m_RestartButton.onClick.RemoveAllListeners();
    }

    public override void SetScore(string score)
    {
        m_ScoreDisplay.text = score;
    }

    public override void SetWinner(string winner)
    {
        m_WinnerDisplay.text = winner;
    }
}
