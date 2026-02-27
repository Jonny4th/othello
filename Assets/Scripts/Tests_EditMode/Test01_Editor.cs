using Core;
using NUnit.Framework;
using System.Linq;
using UnityEngine;

public class Test01_Editor
{
    GameRules m_GameRule = new();

    /*
      0 1 2 3 4 5 6 7
    7 - - - - - - - - 7
    6 - - - - - - - - 6
    5 - - - - - - - - 5
    4 - - - x o - - - 4
    3 - - - o x - - - 3
    2 - - - - - - - - 2
    1 - - - - - - - - 1
    0 - - - - - - - - 0
      0 1 2 3 4 5 6 7
    */

    [Test]
    public void FindLegalMove_00()
    {
        var state = new BoardStateCreator().Build();
        var legalMoves = m_GameRule.FindLegalMoves(state, Faction.Black);

        /* Black's turn
        7 - - - - - - - -
        6 - - - - - - - -
        5 - - - / - - - -
        4 - - / o x - - -
        3 - - - x o / - -
        2 - - - - / - - -
        1 - - - - - - - -
        0 - - - - - - - -
          0 1 2 3 4 5 6 7
        */
        Debug.Log($"Legal Moves: {string.Join(", ", legalMoves.Select(m => $"({m.Item1},{m.Item2})"))}");

        Assert.IsNotNull(legalMoves);
        Assert.AreEqual(4, legalMoves.Count());
        Assert.IsTrue(legalMoves.Contains((2, 4)));
        Assert.IsTrue(legalMoves.Contains((3, 5)));
        Assert.IsTrue(legalMoves.Contains((4, 2)));
        Assert.IsTrue(legalMoves.Contains((5, 3)));
    }

    [Test]
    public void FindLegalMove_01()
    {
        var state = new BoardStateCreator().Build();

        // Black placed disc at (4, 2), most recently.
        state.LastPlacedDiscCoordinates = new Coordinates(4, 2);

        // Black outflanked White token at (4, 3)
        state.Cells[4, 3] = Faction.Black; 

        var legalMoves = m_GameRule.FindLegalMoves(state, Faction.White);

        /* White's turn
        7 - - - - - - - -
        6 - - - - - - - -
        5 - - - - - - - -
        4 - - - o x / - -
        3 - - - x x - - -
        2 - - - / x / - -
        1 - - - - - - - -
        0 - - - - - - - -
          0 1 2 3 4 5 6 7
        */
        Debug.Log($"Legal Moves: {string.Join(", ", legalMoves.Select(m => $"({m.Item1},{m.Item2})"))}");

        Assert.IsNotNull(legalMoves);
        Assert.AreEqual(3, legalMoves.Count());
        Assert.IsTrue(legalMoves.Contains((3, 2)));
        Assert.IsTrue(legalMoves.Contains((5, 2)));
        Assert.IsTrue(legalMoves.Contains((5, 4)));
    }

    [Test]
    public void TestGetAllOutflankedTokens_01()
    {
        var state = new BoardStateCreator().Build();
        state.LastPlacedDiscCoordinates = new Coordinates(4, 2); // Last placed disc at (4, 2)
        state.Cells[4, 2] = Faction.Black; // Place a black token at (4, 2)

        /* Black played at (4, 2)
        7 - - - - - - - -
        6 - - - - - - - -
        5 - - - - - - - -
        4 - - - o x - - -
        3 - - - x o - - -
        2 - - - - X - - -
        1 - - - - - - - -
        0 - - - - - - - -
          0 1 2 3 4 5 6 7
        */

        var outflankedTokens = m_GameRule.GetAllOutflankedTokens(state);
        Debug.Log($"Legal Moves: {string.Join(", ", outflankedTokens.Select(m => $"({m.Item1},{m.Item2})"))}");

        Assert.IsNotNull(outflankedTokens);
        Assert.AreEqual(1, outflankedTokens.Count());
        Assert.IsTrue(outflankedTokens.Contains((4, 3)));
    }

    [Test]
    public void TestGetAllOutflankedTokens_02()
    {
        var state = new BoardStateCreator().Build();
        state.LastPlacedDiscCoordinates = new Coordinates(4, 0);
        state.Cells[4, 0] = Faction.Black; 
        state.Cells[4, 1] = Faction.White; 
        state.Cells[4, 2] = Faction.White; 

        /* Black outflanked multiple, single-direction tokens
        7 - - - - - - - -
        6 - - - - - - - -
        5 - - - - - - - -
        4 - - - o x - - -
        3 - - - x o - - -
        2 - - - - o- - -
        1 - - - - o - - -
        0 - - - - X - - -
          0 1 2 3 4 5 6 7
        */

        var outflankedTokens = m_GameRule.GetAllOutflankedTokens(state);
        Debug.Log($"Legal Moves: {string.Join(", ", outflankedTokens.Select(m => $"({m.Item1},{m.Item2})"))}");

        Assert.IsNotNull(outflankedTokens);
        Assert.AreEqual(3, outflankedTokens.Count());
        Assert.IsTrue(outflankedTokens.Contains((4, 3)));
        Assert.IsTrue(outflankedTokens.Contains((4, 2)));
        Assert.IsTrue(outflankedTokens.Contains((4, 1)));
    }

    [Test]
    public void TestGetAllOutflankedTokens_03()
    {
        var state = new BoardStateCreator().Build();
        state.LastPlacedDiscCoordinates = new Coordinates(1, 4);
        state.Cells[1, 4] = Faction.Black;
        state.Cells[2, 4] = Faction.White;
        state.Cells[2, 3] = Faction.White;
        state.Cells[3, 3] = Faction.White;
        state.Cells[3, 2] = Faction.White;
        state.Cells[4, 3] = Faction.Black;
        state.Cells[4, 1] = Faction.Black;


        /* Black outflanked multiple tokens in multiple directions
        7 - - - - - - - -
        6 - - - - - - - -
        5 - - - - - - - -
        4 - X o o x - - -
        3 - - o o x - - -
        2 - - - o - - - -
        1 - - - - x - - -
        0 - - - - - - - -
          0 1 2 3 4 5 6 7
        */

        var outflankedTokens = m_GameRule.GetAllOutflankedTokens(state);
        Debug.Log($"Legal Moves: {string.Join(", ", outflankedTokens.Select(m => $"({m.Item1},{m.Item2})"))}");

        Assert.IsNotNull(outflankedTokens);
        Assert.AreEqual(4, outflankedTokens.Count());
        Assert.IsTrue(outflankedTokens.Contains((2, 4)));
        Assert.IsTrue(outflankedTokens.Contains((2, 3)));
        Assert.IsTrue(outflankedTokens.Contains((3, 4)));
        Assert.IsTrue(outflankedTokens.Contains((3, 2)));
    }

    [Test]
    public void TestAllWhite()
    {
        var state = new BoardStateCreator().Build();
        state.LastPlacedDiscCoordinates = new Coordinates(1, 4);
        state.Cells[4, 4] = Faction.White;
        state.Cells[3, 3] = Faction.White;

        /* Simulate all White tokens
        7 - - - - - - - -
        6 - - - - - - - -
        5 - - - - - - - -
        4 - - - o o - - -
        3 - - - o o - - -
        2 - - - - - - - -
        1 - - - - - - - -
        0 - - - - - - - -
          0 1 2 3 4 5 6 7
        */

        var outflankedTokens = m_GameRule.FindLegalMoves(state, Faction.Black);
        Debug.Log($"Legal Moves: {string.Join(", ", outflankedTokens.Select(m => $"({m.Item1},{m.Item2})"))}");

        Assert.IsNotNull(outflankedTokens);
        Assert.AreEqual(0, outflankedTokens.Count());
    }
}
