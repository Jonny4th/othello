using Core;
using Main.Models;
using UnityEngine;
using UnityEngine.UI;

public class BasicStartMenu : BaseStartMenu
{
    [SerializeField]
    private CanvasGroup m_TitlePage;

    [SerializeField]
    private Button m_StartWithBot_Button;

    [SerializeField]
    private Button m_StartWithTwoLocalPlayer_Button;

    [SerializeField]
    private CanvasGroup m_ChooseColorPage;

    [SerializeField]
    private Button m_ChooseBlack_Button;

    [SerializeField]
    private Button m_ChooseWhite_Button;

    [SerializeField]
    private GameConfig m_GameConfigAsset;

    private void OnEnable()
    {
        m_StartWithBot_Button.onClick.AddListener(HandleStartWithBotButtonClicked);
        m_StartWithTwoLocalPlayer_Button.onClick.AddListener(HandleStartWithTwoLocalPlayersButtonClicked);
    }

    private void OnDisable()
    {
        m_StartWithBot_Button.onClick.RemoveListener(HandleStartWithBotButtonClicked);
        m_StartWithTwoLocalPlayer_Button.onClick.RemoveListener(HandleStartWithTwoLocalPlayersButtonClicked);
    }

    private void HandleStartWithTwoLocalPlayersButtonClicked()
    {
        OnPlayGameRequest?.Invoke(new GameParameters { IsBotUsed = false });
    }

    private void HandleStartWithBotButtonClicked()
    {
        GoToChooseColorPage();
    }

    private void GoToChooseColorPage()
    {
        m_TitlePage.alpha = 0;
        m_ChooseColorPage.alpha = 1;
        m_ChooseBlack_Button.onClick.AddListener(() => HandleColorSelected(Faction.Black));
        m_ChooseWhite_Button.onClick.AddListener(() => HandleColorSelected(Faction.White));
    }

    private void HandleColorSelected(Faction playerColor)
    {
        m_ChooseBlack_Button.onClick.RemoveAllListeners();
        m_ChooseWhite_Button.onClick.RemoveAllListeners();
        m_ChooseColorPage.alpha = 0;

        OnPlayGameRequest?.Invoke(new GameParameters { PlayerFaction = playerColor, IsBotUsed = true });
    }
}
