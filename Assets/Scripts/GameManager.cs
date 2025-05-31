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

    public bool IsBlackTurn = true; // Track whose turn it is.

    private Cell[,] m_Cells;

    private GameRules m_GameRules = new();

    private void Start()
    {
        CreateBoard();
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

        ShowHint(Occupancy.Black);
    }

    private void ShowHint(Occupancy player)
    {
        var boardState = new BoardState()
        {
            LastPlacedDiscCoordinates = new(-1, -1),
            Cells = ConvertCellsToTokenMap(m_Cells)
        };

        foreach(var hint in m_GameRules.FindLegalMoves(boardState, player))
        {
            m_Cells[hint.Item1, hint.Item2].ShowHintVisual();
        }
    }

    private void OnCellClicked(ICell cell)
    {
        if(cell.CurrentToken != Occupancy.None)
        {
            return;
        }

        foreach(var c in m_Cells) c.HideHintVisual(); 

        var token = IsBlackTurn ? Occupancy.Black : Occupancy.White;
        cell.SetToken(token);
        IsBlackTurn = !IsBlackTurn;

        var boardState = new BoardState()
        {
            LastPlacedDiscCoordinates = cell.Coordinates,
            Cells = ConvertCellsToTokenMap(m_Cells)
        };

        foreach(var coords in m_GameRules.GetAllOutflankedTokens(boardState))
        {
            m_Cells[coords.Item1, coords.Item2].SetToken(token);
        };
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
