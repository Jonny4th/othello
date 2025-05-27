using System;
using UnityEngine;
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
    Black,
    White,
}

public class Cell : MonoBehaviour
{
    [SerializeField]
    private Coordinates m_Coordinates;
    public Coordinates Coordinates => m_Coordinates;

    [SerializeField]
    private CellStateController m_CellStateController;

    public void SetCoordinates(int x, int y)
    {
        m_Coordinates = new Coordinates(x, y);
    }

    public void Place(Token token)
    {
        //if legal move place token.
        m_CellStateController.UpdateToken(token);
    }

    public void HandleClick(BaseEventData eventData)
    {
        if(eventData is PointerEventData)
        {
            Debug.Log($"2:Cell clicked at coordinates: {m_Coordinates}");
            Place(Token.Black); // Example action, replace with actual game logic
        }
    }
}