using NUnit.Framework;
using System;
using System.Collections.Generic;

internal class TestCell : ICell
{
    public Coordinates Coordinates { get; set; }

    public bool IsOccupied { get; set; }

    public Token CurrentToken { get; set; }

    public event Action<ICell> OnCellClicked;

    public void HideHintVisual()
    {
    }

    public void SetCoordinates(int x, int y)
    {
        Coordinates = new Coordinates(x, y);
    }

    public void SetToken(Token token)
    {
        CurrentToken = token;
        IsOccupied = true;
    }

    public void ShowHintVisual()
    {
    }
}

public class Test01_Editor
{

    // A Test behaves as an ordinary method
    [Test]
    public void Test01_EditorSimplePasses()
    {
        var cells = new Dictionary<(int, int), TestCell>();

        for(int x = 0; x < 8; x++)
        {
            for(int y = 0; y < 8; y++)
            {
                var cell = new TestCell();
                cell.SetCoordinates(x, y);
                cells.Add((x, y), cell);
            }
        }
    }
}
