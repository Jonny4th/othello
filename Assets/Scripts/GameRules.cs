using System.Collections.Generic;
using UnityEngine;

public enum Occupancy
{
    None,
    Black,
    White,
    OutOfBounds,
}

public struct BoardState
{
    public Coordinates LastPlacedDiscCoordinates;
    public Occupancy[,] Cells;
}

public class GameRules
{
    private Occupancy GetDirectionalNeighbour(Vector2Int anchorPoint, Vector2Int direction, BoardState state)
    {
        anchorPoint += direction;

        bool isOutOfBounds = anchorPoint.x < 0 || anchorPoint.x >= state.Cells.GetLength(0) ||
                             anchorPoint.y < 0 || anchorPoint.y >= state.Cells.GetLength(1);

        if(isOutOfBounds) return Occupancy.OutOfBounds;

        return state.Cells[anchorPoint.x, anchorPoint.y];
    }

    private int GetPossibleDirectionalCaptureAmount(Occupancy player, Vector2Int anchorPoint, Vector2Int direction, BoardState state)
    {
        // first neighbour must be opponent's token
        if(!IsFirstNeigbourOpponent()) return 0;

        anchorPoint += direction;
        var captureAmount = 1;

        while(true)
        {
            var neighbour = GetDirectionalNeighbour(anchorPoint, direction, state);

            if(neighbour == Occupancy.OutOfBounds || neighbour == player)
                return 0;
            if(neighbour == Occupancy.None)
                return captureAmount + 1;

            captureAmount++;
            anchorPoint += direction;
        }

        bool IsFirstNeigbourOpponent()
        {
            var neighbour = GetDirectionalNeighbour(anchorPoint, direction, state);
            return neighbour != Occupancy.None && neighbour != Occupancy.OutOfBounds && neighbour != player;
        }
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

    public (int, int)[] FindLegalMoves(BoardState state, Occupancy player)
    {
        var legalMoves = new List<(int, int)>();

        for(int i = 0; i < state.Cells.GetLength(0); i++)
        {
            for(int j = 0; j < state.Cells.GetLength(1); j++)
            {
                if(state.Cells[i, j] == Occupancy.None) continue;
                if(state.Cells[i, j] != player) continue;

                foreach(var direction in directions)
                {
                    int captureAmount = GetPossibleDirectionalCaptureAmount(player, new Vector2Int(i, j), direction, state);

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
