using Core;
using Main.Models;
using UnityEngine;
using UnityEngine.UI;

public class BasicStartMenu : BaseStartMenu
{
    [SerializeField]
    private GameObject m_TitlePage;

    [SerializeField]
    private Button m_StartWithBot_Button;

    [SerializeField]
    private Button m_StartWithTwoLocalPlayer_Button;

    [SerializeField]
    private GameObject m_ChooseColorPage;

    [SerializeField]
    private Button m_ChooseBlack_Button;

    [SerializeField]
    private Button m_ChooseWhite_Button;

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
        m_TitlePage.SetActive(false);
        m_ChooseColorPage.SetActive(true);
        m_ChooseBlack_Button.onClick.AddListener(() => HandleColorSelected(Occupancy.Black));
        m_ChooseWhite_Button.onClick.AddListener(() => HandleColorSelected(Occupancy.White));
    }

    private void HandleColorSelected(Occupancy playerColor)
    {
        m_ChooseBlack_Button.onClick.RemoveAllListeners();
        m_ChooseWhite_Button.onClick.RemoveAllListeners();
        m_ChooseColorPage.SetActive(false);

        OnPlayGameRequest?.Invoke(new GameParameters { PlayerColor = playerColor, IsBotUsed = true });
    }
}
