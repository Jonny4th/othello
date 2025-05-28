using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[Serializable]
public struct Coordinates
{
    public int X;
    public int Y;

    public Coordinates(int x, int y)
    {
        X = x;
        Y = y;
    }
    
    public override string ToString()
    {
        return $"({X}, {Y})";
    }
}

public enum Token
{
    None,
    Black,
    White,
}

public class Cell : MonoBehaviour, ICell
{
    [SerializeField]
    private Coordinates m_Coordinates;
    public Coordinates Coordinates => m_Coordinates;

    [SerializeField]
    private CellVisualController m_CellStateController;

    public Token CurrentToken => m_CurrentToken;
    private Token m_CurrentToken = Token.None;

    private bool m_IsOccupied => m_CurrentToken != Token.None;

    public event Action<ICell> OnCellClicked;

    public void SetCoordinates(int x, int y)
    {
        m_Coordinates = new Coordinates(x, y);
    }

    public void SetToken(Token token)
    {
        if(m_IsOccupied)
        {
            Debug.LogWarning($"Cell at {m_Coordinates} is already occupied. Cannot place token: {token}");
            return;
        }

        //if legal move place token.
        
        m_CurrentToken = token;
        m_CellStateController.SetToken(token);
    }

    public void ShowHintVisual()
    {
        if(m_IsOccupied)
        {
            Debug.LogWarning($"Cell at {m_Coordinates} is already occupied. Cannot show hint visual.");
            return;
        }

        m_CellStateController.ShowHintVisual();
    }

    public void HideHintVisual()
    {
        if(m_IsOccupied)
        {
            Debug.LogWarning($"Cell at {m_Coordinates} is already occupied. Cannot hide hint visual.");
            return;
        }

        m_CellStateController.HideHintVisual();
    }

    /// <summary>
    /// For Event Trigger component to handle clicks on the cell.
    /// </summary>
    /// <param name="eventData"></param>
    public void HandleClick(BaseEventData eventData)
    {
        if(eventData is PointerEventData)
        {
            if(m_IsOccupied)
            {
                Debug.LogWarning($"Cell at {m_Coordinates} is already occupied. Cannot click.");
                return;
            }

            OnCellClicked?.Invoke(this);
        }
    }
}

public interface ICell
{
    Coordinates Coordinates { get; }
    Token CurrentToken { get; }

    event Action<ICell> OnCellClicked;

    void HideHintVisual();
    void SetCoordinates(int x, int y);
    void SetToken(Token token);
    void ShowHintVisual();
}