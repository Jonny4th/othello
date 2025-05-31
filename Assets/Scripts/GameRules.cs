using System;
using System.Collections.Generic;
using UnityEngine;

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

    public Vector2Int ToVector2Int()
    {
        return new Vector2Int(X, Y);
    }

    public override string ToString()
    {
        return $"({X}, {Y})";
    }
}

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

    public (int, int)[] GetAllOutflankedTokens(BoardState state)
    {
        var capturedTokens = new List<(int, int)>();

        Vector2Int anchorPoint = state.LastPlacedDiscCoordinates.ToVector2Int();
        Occupancy player = state.Cells[anchorPoint.x, anchorPoint.y];

        foreach(var direction in directions)
        {
            int captureAmount = GetTrueDirectionalCaptureAmount(player, anchorPoint, direction, state);

            if(captureAmount == 0) continue;

            for(int i = 1; i <= captureAmount; i++)
            {
                var captured = anchorPoint + direction * i;
                capturedTokens.Add((captured.x, captured.y));
            }
        }

        return capturedTokens.ToArray();
    }

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
        return GetDirectionalCaptureAmount(player, anchorPoint, direction, state, outflanking: false);
    }

    private int GetTrueDirectionalCaptureAmount(Occupancy player, Vector2Int anchorPoint, Vector2Int direction, BoardState state)
    {
        return GetDirectionalCaptureAmount(player, anchorPoint, direction, state, outflanking: true);
    }

    private int GetDirectionalCaptureAmount(Occupancy player, Vector2Int anchorPoint, Vector2Int direction, BoardState state, bool outflanking)
    {
        // first neighbour must be opponent's token
        if(!IsFirstNeigbourOpponent()) return 0;

        anchorPoint += direction;
        var captureAmount = 1;

        while(true)
        {
            var neighbour = GetDirectionalNeighbour(anchorPoint, direction, state);

            if(outflanking)
            {
                if(neighbour == Occupancy.OutOfBounds || neighbour == Occupancy.None)
                    return 0;
                if(neighbour == player)
                    return captureAmount;
            }
            else
            {
                if(neighbour == Occupancy.OutOfBounds || neighbour == player)
                    return 0;
                if(neighbour == Occupancy.None)
                    return captureAmount + 1;
            }

            captureAmount++;
            anchorPoint += direction;
        }

        bool IsFirstNeigbourOpponent()
        {
            var neighbour = GetDirectionalNeighbour(anchorPoint, direction, state);
            return neighbour != Occupancy.None && neighbour != Occupancy.OutOfBounds && neighbour != player;
        }
    }
}
