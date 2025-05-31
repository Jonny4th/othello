using System.Linq;
using UnityEngine;

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

    //[SerializeField]
    //private TMP_Text m_CurrentTurnPlayer;

    public bool IsBlackTurn = true; // Track whose turn it is.
    
    [SerializeField]
    private bool m_IsGameOver = false;
    public bool IsGameOver => m_IsGameOver;


    private Cell[,] m_Cells;

    private GameRules m_GameRules = new();

    private void Start()
    {
        StartGame();
    }

    private void CreateBoard()
    {
        m_Cells = BoardCreator.CreateBoard(m_CellPrototype, m_BoardWidth, m_BoardHeight, m_BoardParent);
        Debug.Log($"Board created with {m_Cells.Length} cells.");

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

    public void StartGame()
    {
        CreateBoard();
        IsBlackTurn = true;
        TurnPhase();
    }

    public void TurnPhase()
    {
        ShowHint(IsBlackTurn ? Occupancy.Black : Occupancy.White);
    }

    (int, int)[] m_CurrentLegalMoves;

    private void ShowHint(Occupancy player)
    {
        var boardState = new BoardState()
        {
            LastPlacedDiscCoordinates = new(-1, -1),
            Cells = ConvertCellsToTokenMap(m_Cells)
        };

        m_CurrentLegalMoves = m_GameRules.FindLegalMoves(boardState, player);

        if(m_CurrentLegalMoves.Length == 0)
        {
            IsBlackTurn = !IsBlackTurn;
            TurnPhase();
        }

        foreach((int x, int y) in m_CurrentLegalMoves)
        {
            m_Cells[x, y].ShowHintVisual();
        }
    }

    private void OnCellClicked(ICell cell)
    {
        if(cell.CurrentToken != Occupancy.None) return;

        if(!m_CurrentLegalMoves.Contains(cell.Coordinates.ToTuple())) return;

        foreach(var c in m_Cells) c.HideHintVisual();

        var token = IsBlackTurn ? Occupancy.Black : Occupancy.White;
        cell.SetToken(token);
        IsBlackTurn = !IsBlackTurn;

        var boardState = new BoardState()
        {
            LastPlacedDiscCoordinates = cell.Coordinates,
            Cells = ConvertCellsToTokenMap(m_Cells)
        };

        foreach((int x, int y) in m_GameRules.GetAllOutflankedTokens(boardState))
        {
            m_Cells[x, y].SetToken(token);
        };

        boardState.Cells = ConvertCellsToTokenMap(m_Cells);

        if(m_GameRules.IsGameOver(boardState))
        {
            (int a, int b) = m_GameRules.CountTokens(boardState);
            var winner = a > b ? Occupancy.Black : Occupancy.White;
            if(a == b) { winner = Occupancy.None; }

            Debug.Log($"Game Over: {winner} won.\nBlack / White:{a}/{b}");
            return;
        }
     
        TurnPhase();
    }

    public Occupancy[,] ConvertCellsToTokenMap(Cell[,] cells)
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
