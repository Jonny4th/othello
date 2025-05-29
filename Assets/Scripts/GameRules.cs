using System.Collections.Generic;
using UnityEngine;

public struct BoardState
{
    public Coordinates LastPlacedDiscCoordinates;
    public Token[,] Cells;
}

public class GameRules
{
    private Token GetDirectionalNeighbour((int x, int y) anchorPoint, Vector2Int direction, BoardState state)
    {
        int x = anchorPoint.x + direction.x;
        int y = anchorPoint.y + direction.y;

        if(x < 0 || x >= state.Cells.GetLength(0) || y < 0 || y >= state.Cells.GetLength(1))
            return Token.None;

        return state.Cells[x, y];
    }

    private int GetDirectionalCaptureAmount(Token player, (int x, int y) anchorPoint, Vector2Int direction, BoardState state)
    {
        Token neighbour = GetDirectionalNeighbour(anchorPoint, direction, state);

        if(neighbour == Token.None || neighbour == player) return 0;

        int x = anchorPoint.x + direction.x;
        int y = anchorPoint.y + direction.y;
        var captureAmount = 1;

        while(x >= 0 && x < state.Cells.GetLength(0) && y >= 0 && y < state.Cells.GetLength(1))
        {
            Token current = state.Cells[x, y];

            if(current == Token.None) return captureAmount;
            if(current == player) return 0;

            x += (int)direction.x;
            y += (int)direction.y;
            captureAmount++;
        }

        return 0;
    }

    private readonly Vector2Int[] directions = new Vector2Int[]
    {
        new (0, 1), // Up
        new (0, -1), // Down
        new (-1, 0),// Left
        new (1, 0), // Right
        new (-1, 1),// Up-Left
        new (1, 1), // Up-Right
        new (-1, -1),// Down-Left
        new (1, -1),// Down-Right
    };

    public (int, int)[] FindLegalMoves(BoardState state, Token player)
    {
        var legalMoves = new List<(int, int)>();

        for(int i = 0; i < state.Cells.GetLength(0); i++)
        {
            for(int j = 0; j < state.Cells.GetLength(1); j++)
            {
                if(state.Cells[i, j] == Token.None) continue;
                if(state.Cells[i, j] != player) continue;

                foreach(var direction in directions)
                {
                    int captureAmount = GetDirectionalCaptureAmount(player, (i, j), direction, state);

                    if(captureAmount == 0) continue;

                    int targetX = i + direction.x * captureAmount;
                    int targetY = j + direction.y * captureAmount;
                        
                    if(legalMoves.Contains((targetX, targetY))) continue;

                    legalMoves.Add((targetX, targetY));
                }
            }
        }

        return legalMoves.ToArray();
    }

    public (int, int)[] FindAllCapturedToken(BoardState state)
    {
        return new (int, int)[0];
    }
}
