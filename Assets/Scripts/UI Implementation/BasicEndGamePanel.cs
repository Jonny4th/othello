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

    [SerializeField]
    private ButtonWithText m_ViewToggleButton;

    [SerializeField]
    private CanvasRenderer m_CanvasRenderer;

    private void OnEnable()
    {
        m_RestartButton.onClick.AddListener(() => OnRestartButtonPressed?.Invoke());
    }

    private void OnDisable()
    {
        m_RestartButton.onClick.RemoveAllListeners();
    }

    public override void Show()
    {
        base.Show();
        ShowDetails();
    }

    public override void SetScore(string score)
    {
        m_ScoreDisplay.text = score;
    }

    public override void SetWinner(string winner)
    {
        m_WinnerDisplay.text = winner;
    }

    private void ShowDetails()
    {
        m_CanvasRenderer.SetAlpha(1);
        m_WinnerDisplay.gameObject.SetActive(true);
        m_ScoreDisplay.gameObject.SetActive(true);
        m_ViewToggleButton.Text = "View Board";
        m_ViewToggleButton.onClick.RemoveListener(ShowDetails);
        m_ViewToggleButton.onClick.AddListener(ShowBoard);
    }

    private void ShowBoard()
    {
        m_CanvasRenderer.SetAlpha(0);
        m_WinnerDisplay.gameObject.SetActive(false);
        m_ScoreDisplay.gameObject.SetActive(false);
        m_ViewToggleButton.Text = "View Score";
        m_ViewToggleButton.onClick.RemoveListener(ShowBoard);
        m_ViewToggleButton.onClick.AddListener(ShowDetails);
    }
}
