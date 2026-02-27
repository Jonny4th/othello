using Core;
using Main.Models;
using Main.UIs;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Main.GameManager
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private BaseStartMenu m_StartMenu;

        [SerializeField]
        private BaseEndGameDisplay m_EndGameDisplay;

        public bool IsBlackTurn = true; // Track whose turn it is.

        [SerializeField]
        private bool m_IsGameOver = false;
        public bool IsGameOver => m_IsGameOver;

        private GameRules m_GameRules = new();

        [SerializeField]
        private string m_WhitePlayerName = "Chandra";

        [SerializeField]
        private string m_BlackPlayerName = "Rahu";

        [SerializeField]
        private Faction m_BotColor = Faction.None; // bot occupancy = none to ensure bot is not used by default

        [Header("Events")]
        [SerializeField]
        private BoardStateEvent m_EndGameEvent;

        [SerializeField]
        private GameParametersEvent m_StartGameEvent;

        private GameParameters m_GameParameters;

        private void Start()
        {
            ShowTitlePage();
            m_EndGameEvent.AddListener(TriggerEndGame);
            m_EndGameDisplay.Hide();
            m_EndGameDisplay.OnRestartButtonPressed.AddListener(HandleRestart);
        }

        private void HandleRestart()
        {
            BeginGame(m_GameParameters);
        }

        private void ShowTitlePage()
        {
            m_StartMenu.OnPlayGameRequest.AddListener(BeginGame);
            m_StartMenu.Show();
        }

        private async void BeginGame(GameParameters parameter)
        {
            //show loading

            await SceneManager.LoadSceneAsync(1,LoadSceneMode.Additive);

            //hide loading

            m_StartMenu.OnPlayGameRequest.RemoveListener(BeginGame);

            m_StartMenu.Hide();
            m_EndGameDisplay.Hide();

            m_GameParameters = new(
                faction: parameter.PlayerFaction,
                isBotUsed: parameter.IsBotUsed, 
                blackPlayer: m_BlackPlayerName, 
                whitePlayer: m_WhitePlayerName);

            m_StartGameEvent.Invoke(parameter);
        }

        public void TriggerEndGame(BoardState boardState)
        {
            (int a, int b) = m_GameRules.CountTokens(boardState);
            var winner = a > b ? Faction.Black : Faction.White;
            if (a == b) { winner = Faction.None; }

            switch (winner)
            {
                case Faction.None:
                    m_EndGameDisplay.SetWinner("It's a draw!\n<size=120>Wanna try another round?</size>");
                    break;
                case Faction.Black:
                    m_EndGameDisplay.SetWinner("Rahu won!\n<size=120>Come and be eaten, Chandra!</size>");
                    break;
                case Faction.White:
                    m_EndGameDisplay.SetWinner("Chandra won!\n<size=120>Leave me alone already, Rahu!</size>");
                    break;
            }

            m_EndGameDisplay.SetScore($"Rahu {a}\nChandra {b}");

            m_EndGameDisplay.Show();
        }
    }
}