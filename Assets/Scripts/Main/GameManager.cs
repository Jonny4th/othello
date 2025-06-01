using Core;
using Main.Models;
using Main.UIs;
using System.Linq;
using UnityEngine;

namespace Main.GameManager
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private Cell m_CellPrototype;

        [SerializeField]
        private int m_BoardWidth = 8;

        [SerializeField]
        private int m_BoardHeight = 8;

        [SerializeField]
        private Transform m_BoardParent;

        [SerializeField]
        private BaseStartMenu m_StartMenu;

        [SerializeField]
        private BaseEndGameDisplay m_EndGameDisplay;

        [SerializeField]
        private BaseHUD m_HUD;

        public bool IsBlackTurn = true; // Track whose turn it is.

        [SerializeField]
        private bool m_IsGameOver = false;
        public bool IsGameOver => m_IsGameOver;

        (int, int)[] m_CurrentLegalMoves;

        private Cell[,] m_Cells;

        private GameRules m_GameRules = new();

        [SerializeField]
        private string m_WhitePlayerName = "Chandra";

        [SerializeField]
        private string m_BlackPlayerName = "Rahu";

        private Bot m_Bot;
        private bool m_IsBotTurn = false;

        [SerializeField]
        private Occupancy m_BotColor = Occupancy.None; // bot occupancy = none to ensure bot is not used by default

        private void Start()
        {
            ShowTitlePage();
            CreateBoard();
        }

        private void ShowTitlePage()
        {
            m_StartMenu.OnPlayGameRequest.AddListener(BeginGame);
            m_StartMenu.Show();
        }

        private void BeginGame(GameParameters parameter)
        {
            m_StartMenu.OnPlayGameRequest.RemoveListener(BeginGame);

            if(parameter.IsBotUsed)
            {
                m_Bot = new();
                m_BotColor = parameter.PlayerColor == Occupancy.Black ? Occupancy.White : Occupancy.Black;
                m_Bot.SetGameRules(m_GameRules);
                m_Bot.OnBotMoveMade += HandleBotMoveMade;
            }

            StartGame();
        }

        public void StartGame()
        {
            if(m_Bot != null) m_Bot.OnBotMoveMade += HandleBotMoveMade;

            m_StartMenu.Hide();
            m_EndGameDisplay.Hide();

            CreateBoard();

            IsBlackTurn = true;

            m_HUD.SetPlayerName(m_BlackPlayerName, m_WhitePlayerName);
            m_HUD.SetScore(2, 2);
            m_HUD.SetPlayerTurn(Occupancy.Black);
            m_HUD.Show();

            TurnPhase();
        }

        public void TurnPhase()
        {
            var currentPlayer = IsBlackTurn ? Occupancy.Black : Occupancy.White;
            m_HUD.SetPlayerTurn(currentPlayer);
            m_IsBotTurn = currentPlayer == m_BotColor && !IsGameOver;

            var boardState = new BoardState()
            {
                LastPlacedDiscCoordinates = new(-1, -1),
                Cells = ConvertCellsToTokenMap(m_Cells)
            };

            ShowHint(boardState, currentPlayer);

            if(m_IsBotTurn & !m_IsGameOver)
            {
                m_Bot.MakeDecision(boardState, m_BotColor);
            }
        }

        public void TriggerEndGame(BoardState boardState)
        {
            if(m_Bot != null) m_Bot.OnBotMoveMade -= HandleBotMoveMade;

            (int a, int b) = m_GameRules.CountTokens(boardState);
            var winner = a > b ? Occupancy.Black : Occupancy.White;
            if(a == b) { winner = Occupancy.None; }

            switch(winner)
            {
                case Occupancy.None:
                    m_EndGameDisplay.SetWinner("It's a draw!\n<size=120>Wanna try another round?</size>");
                    break;
                case Occupancy.Black:
                    m_EndGameDisplay.SetWinner("Rahu won!\n<size=120>Come and be eaten, Chandra!</size>");
                    break;
                case Occupancy.White:
                    m_EndGameDisplay.SetWinner("Chandra won!\n<size=120>Leave me alone already, Rahu!</size>");
                    break;
            }

            m_EndGameDisplay.SetScore($"Rahu {a}\nChandra {b}");

            m_HUD.Hide();

            m_EndGameDisplay.Show();
        }

        private void CreateBoard()
        {
            m_Cells ??= BoardCreator.CreateBoard(m_CellPrototype, m_BoardWidth, m_BoardHeight, m_BoardParent);

            var initialState = new BoardStateCreator().Build();

            for(int i = 0; i < m_BoardWidth; i++)
            {
                for(int j = 0; j < m_BoardHeight; j++)
                {
                    m_Cells[i, j].SetToken(initialState.Cells[i, j]);
                }
            }

            foreach(var cell in m_Cells)
            {
                cell.OnCellClicked += OnCellClicked;
            }
        }

        private void ShowHint(BoardState boardState, Occupancy player)
        {
            m_CurrentLegalMoves = m_GameRules.FindLegalMoves(boardState, player);

            if(m_CurrentLegalMoves.Length == 0) SwitchTurn();

            foreach((int x, int y) in m_CurrentLegalMoves)
            {
                m_Cells[x, y].ShowHintVisual();
            }
        }

        private void OnCellClicked(ICell cell)
        {
            if(m_IsBotTurn) return; //block player from clicking while bot is making a move
            ProcessMove(cell);
        }

        private void ProcessMove(ICell cell)
        {
            if(cell.CurrentToken != Occupancy.None) return;

            if(!m_CurrentLegalMoves.Contains(cell.Coordinates.ToTuple())) return;

            foreach(var c in m_Cells) c.HideHintVisual();

            var token = IsBlackTurn ? Occupancy.Black : Occupancy.White;
            m_Cells[cell.Coordinates.X, cell.Coordinates.Y].SetToken(token);

            var boardState = new BoardState()
            {
                LastPlacedDiscCoordinates = cell.Coordinates,
                Cells = ConvertCellsToTokenMap(m_Cells)
            };

            var updatedBoardState = Resolve(boardState);

            if(m_GameRules.IsGameOver(updatedBoardState))
            {
                TriggerEndGame(updatedBoardState);
                return;
            }
            
            SwitchTurn();
        }

        private void HandleBotMoveMade(Coordinates coordinates)
        {
            Debug.Log($"Bot made a move at {coordinates}");
            if(coordinates.X < 0 || coordinates.Y < 0 || coordinates.X >= m_BoardWidth || coordinates.Y >= m_BoardHeight)
            {
                Debug.LogWarning("Bot made an invalid move. Ignoring.");
                SwitchTurn();
                return;
            }

            ICell cell = m_Cells[coordinates.X, coordinates.Y];
            ProcessMove(cell);
        }

        private void SwitchTurn()
        {
            IsBlackTurn = !IsBlackTurn;
            m_IsBotTurn &= false;
            TurnPhase();
        }

        private BoardState Resolve(BoardState boardState)
        {
            foreach((int x, int y) in m_GameRules.GetAllOutflankedTokens(boardState))
            {
                Debug.Log($"{x}{y}");
                m_Cells[x, y].SetToken(boardState.LastPlacedDisc);
            }

            boardState.Cells = ConvertCellsToTokenMap(m_Cells);

            (int a, int b) = m_GameRules.CountTokens(boardState);
            m_HUD.SetScore(a, b);

            return boardState;
        }

        public Occupancy[,] ConvertCellsToTokenMap(ICell[,] cells)
        {
            var lenX = cells.GetLength(0);
            var lenY = cells.GetLength(1);

            var result = new Occupancy[lenX, lenY];

            foreach(var cell in cells)
            {
                result[cell.Coordinates.X, cell.Coordinates.Y] = cell.CurrentToken;
            }

            return result;
        }
    }
}