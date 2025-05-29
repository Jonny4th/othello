using System.Collections.Generic;
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
                m_Cells[i,j].SetToken(initialState.Cells[i,j]);
            }
        }

        foreach(var cell in m_Cells)
        {
            cell.OnCellClicked += OnCellClicked;
        }

        var boardState = new BoardState()
        {
            LastPlacedDiscCoordinates = new(-1,-1),
            Cells = ConvertCellsToTokenMap(m_Cells)
        };
        
        foreach(var hint in m_GameRules.FindLegalMoves(boardState, Token.Black))
        {
            m_Cells[hint.Item1, hint.Item2].ShowHintVisual();
        }
    }

    private void OnCellClicked(ICell cell)
    {
        if(cell.CurrentToken != Token.None)
        {
            return;
        }

        var token = IsBlackTurn ? Token.Black : Token.White;
        cell.SetToken(token);
        IsBlackTurn = !IsBlackTurn;

        var boardState = new BoardState()
        {
            LastPlacedDiscCoordinates = cell.Coordinates,
            Cells = ConvertCellsToTokenMap(m_Cells)
        };
    }

    public Token[,] ConvertCellsToTokenMap(Cell[,] cells)
    {
        var lenX = cells.GetLength(0);
        var lenY = cells.GetLength(1);

        var result = new Token[lenX, lenY];

        foreach(var cell in cells)
        {
            result[cell.Coordinates.X, cell.Coordinates.Y] = cell.CurrentToken;
        }

        return result;
    }
}
