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

    private void Start()
    {
        CreateBoard();
    }

    private void CreateBoard()
    {
        var cells = BoardCreator.CreateBoard(m_CellPrototype, m_BoardWidth, m_BoardHeight, m_BoardParent);
        Debug.Log($"Board created with {cells.Length} cells.");

        cells[3,3].SetToken(Token.White);
        cells[4,4].SetToken(Token.White);
        cells[3,4].SetToken(Token.Black);
        cells[4,3].SetToken(Token.Black);

        foreach(var cell in cells)
        {
            cell.OnCellClicked += OnCellClicked;
        }
    }

    private void OnCellClicked(ICell cell)
    {
        var token = IsBlackTurn ? Token.Black : Token.White;
        cell.SetToken(token);
        IsBlackTurn = !IsBlackTurn;
    }
}
