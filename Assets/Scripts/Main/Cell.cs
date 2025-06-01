using Core;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, ICell
{
    [SerializeField]
    private Coordinates m_Coordinates;
    public Coordinates Coordinates => m_Coordinates;

    [SerializeField]
    private CellVisualController m_CellStateController;

    public Occupancy CurrentToken => m_CurrentToken;
    private Occupancy m_CurrentToken = Occupancy.None;

    private bool m_IsOccupied => m_CurrentToken != Occupancy.None;

    public event Action<ICell> OnCellClicked;

    public void SetCoordinates(int x, int y)
    {
        m_Coordinates = new Coordinates(x, y);
    }

    public void SetToken(Occupancy token)
    {
        m_CurrentToken = token;
        m_CellStateController.SetToken(token);
    }

    public void ShowHintVisual()
    {
        if(m_IsOccupied) return;

        m_CellStateController.ShowHintVisual();
    }

    public void HideHintVisual()
    {
        if(m_IsOccupied) return;

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
    Occupancy CurrentToken { get; }

    event Action<ICell> OnCellClicked;

    void HideHintVisual();
    void SetCoordinates(int x, int y);
    void SetToken(Occupancy token);
    void ShowHintVisual();
}