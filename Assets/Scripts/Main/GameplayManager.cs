using Core;
using Main.Models;
using System.Collections;
using System.Linq;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [Header("Events")]
    [SerializeField]
    private BoardStateEvent m_EndGameEvent;

    [SerializeField]
    private GameParametersEvent m_StartGameEvent;

    [Header("Gameplay")]
    [SerializeField]
    private Cell m_CellPrototype;

    [SerializeField]
    private int m_BoardWidth = 8;

    [SerializeField]
    private int m_BoardHeight = 8;

    [SerializeField]
    private Transform m_BoardParent;

    [Header("UIs")]
    [SerializeField]
    private BaseHUD m_Hud;

    private Cell[,] m_Cells;
    private bool m_IsPlaying = false;

    private Coroutine m_Game;
    private GameRules m_GameRules = new();
    (int, int)[] m_CurrentLegalMoves;

    private Bot m_Bot;
    private bool m_IsBotTurn = false;
    private Faction m_BotColor = Faction.None; // bot occupancy = none to ensure bot is not used by default

    public bool IsBlackTurn = true; // Track whose turn it is.

    private void Awake()
    {
        m_StartGameEvent.AddListener(StartGame);
    }

    public void StartGame(GameParameters parameters)
    {
        if(m_Game != null || m_IsPlaying)
        {
            Debug.LogWarning("Cannot start another game while a session is running.");
            return;
        }

        //reset
        m_IsPlaying = true;
        if(m_Cells != null)
        {
            foreach(var cell in m_Cells)
            {
                cell.SetToken(Faction.None);
            }
        }

        //m_Game = StartCoroutine(RunGameLogic(parameters));

        if(parameters.IsBotUsed)
        {
            m_Bot = new();
            m_BotColor = parameters.PlayerFaction == Faction.Black ? Faction.White : Faction.Black;
            m_Bot.SetGameRules(m_GameRules);
            m_Bot.OnBotMoveMade += HandleBotMoveMade;
        }

        Faction playerFaction = parameters.PlayerFaction;
        //set UIs
        m_Hud.SetPlayerName(parameters.BlackPlayer, parameters.WhitePlayer);
        m_Hud.SetScore(2, 2);
        m_Hud.SetPlayerTurn(Faction.Black);
        m_Hud.Show();

        CreateBoard();
        IsBlackTurn = true;

        TurnPhase();
    }

    public void TurnPhase()
    {
        var currentPlayer = IsBlackTurn ? Faction.Black : Faction.White;
        m_Hud.SetPlayerTurn(currentPlayer);
        m_IsBotTurn = currentPlayer == m_BotColor && m_IsPlaying;

        var boardState = new BoardState()
        {
            LastPlacedDiscCoordinates = new(-1, -1),
            Cells = ConvertCellsToTokenMap(m_Cells)
        };

        ShowHint(boardState, currentPlayer);

        if(m_Bot != null && m_IsBotTurn && m_IsPlaying)
        {
            m_Bot.MakeDecision(boardState, m_BotColor);
        }
    }

    public void TriggerEndGame(BoardState boardState)
    {
        if(m_Bot != null) m_Bot.OnBotMoveMade -= HandleBotMoveMade;

        (int a, int b) = m_GameRules.CountTokens(boardState);
        var winner = a > b ? Faction.Black : Faction.White;
        if(a == b) { winner = Faction.None; }

        m_EndGameEvent.Invoke(boardState);

        m_Hud.Hide();
    }

    private IEnumerator RunGameLogic(GameParameters parameters)
    {
        //pre-game
        if(parameters.IsBotUsed)
        {
            m_Bot = new();
            m_BotColor = parameters.PlayerFaction == Faction.Black ? Faction.White : Faction.Black;
            m_Bot.SetGameRules(m_GameRules);
            m_Bot.OnBotMoveMade += HandleBotMoveMade;
        }

        CreateBoard();
        IsBlackTurn = true;

        Faction playerFaction = parameters.PlayerFaction;
        //set UIs
        m_Hud.SetPlayerName(parameters.BlackPlayer, parameters.WhitePlayer);
        m_Hud.SetScore(2, 2);
        m_Hud.SetPlayerTurn(Faction.Black);
        m_Hud.Show();

        //run game
        while(m_IsPlaying)
        {
            yield return null;
        }

        //post game
        TriggerEndGame(new());
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


    private void ShowHint(BoardState boardState, Faction player)
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
        if(m_Bot != null && m_IsBotTurn) return; //block player from clicking while bot is making a move
        ProcessMove(cell);
    }

    private void ProcessMove(ICell cell)
    {
        if(cell.CurrentToken != Faction.None) return;

        if(!m_CurrentLegalMoves.Contains(cell.Coordinates.ToTuple())) return;

        foreach(var c in m_Cells) c.HideHintVisual();

        var token = IsBlackTurn ? Faction.Black : Faction.White;
        m_Cells[cell.Coordinates.X, cell.Coordinates.Y].SetToken(token);

        var boardState = new BoardState()
        {
            LastPlacedDiscCoordinates = cell.Coordinates,
            Cells = ConvertCellsToTokenMap(m_Cells)
        };

        var updatedBoardState = Resolve(boardState);

        if(m_GameRules.IsGameOver(updatedBoardState))
        {
            m_IsPlaying = false;
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
        m_Hud.SetScore(a, b);

        return boardState;
    }

    public Faction[,] ConvertCellsToTokenMap(ICell[,] cells)
    {
        var lenX = cells.GetLength(0);
        var lenY = cells.GetLength(1);

        var result = new Faction[lenX, lenY];

        foreach(var cell in cells)
        {
            result[cell.Coordinates.X, cell.Coordinates.Y] = cell.CurrentToken;
        }

        return result;
    }
}
