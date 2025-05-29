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
    public void InittialStateLegalMoves()
    {
        var state = new BoardStateCreator().Build();
        var legalMoves = m_GameRule.FindLegalMoves(state, Token.Black);

        /*
        7 - - - - - - - -
        6 - - - - - - - -
        5 - - - - / - - -
        4 - - - x o / - -
        3 - - / o x - - -
        2 - - - / - - - -
        1 - - - - - - - -
        0 - - - - - - - -
          0 1 2 3 4 5 6 7
        */

        Assert.IsNotNull(legalMoves);
        Assert.IsTrue(legalMoves.Contains((2,3)));
        Assert.IsTrue(legalMoves.Contains((3,2)));
        Assert.IsTrue(legalMoves.Contains((5,4)));
        Assert.IsTrue(legalMoves.Contains((4,5)));
    }
}
