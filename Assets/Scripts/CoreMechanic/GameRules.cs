using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
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

        public (int, int) ToTuple()
        {
            return (X, Y);
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }

    public enum Faction
    {
        None,
        Black,
        White,
        OutOfBounds,
    }

    public struct BoardState
    {
        public Coordinates LastPlacedDiscCoordinates;
        public Faction[,] Cells;

        public Faction LastPlacedDisc => Cells[LastPlacedDiscCoordinates.X, LastPlacedDiscCoordinates.Y];
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

        public (int black, int white) CountTokens(BoardState state)
        {
            int blackCount = 0;
            int whiteCount = 0;

            for(int i = 0; i < state.Cells.GetLength(0); i++)
            {
                for(int j = 0; j < state.Cells.GetLength(1); j++)
                {
                    switch(state.Cells[i, j])
                    {
                        case Faction.Black:
                            blackCount++;
                            break;
                        case Faction.White:
                            whiteCount++;
                            break;
                    }
                }
            }

            return (blackCount, whiteCount);
        }

        public bool IsGameOver(BoardState state)
        {
            return isBoardFull(state) || isOverTaken(state) || isNoLegalMovesPossible(state);

            bool isOverTaken(BoardState state)
            {
                (int b, int w) = CountTokens(state);
                return b == 0 || w == 0;
            }

            bool isBoardFull(BoardState state)
            {
                foreach(var cell in state.Cells)
                {
                    if(cell == Faction.None) return false;
                }

                return true;
            }

            bool isNoLegalMovesPossible(BoardState state)
            {
                var blackMoves = FindLegalMoves(state, Faction.Black);
                var whiteMoves = FindLegalMoves(state, Faction.White);
                return blackMoves.Length == 0 && whiteMoves.Length == 0;
            }
        }

        public (int, int)[] FindLegalMoves(BoardState state, Faction player)
        {
            var legalMoves = new List<(int, int)>();

            for(int i = 0; i < state.Cells.GetLength(0); i++)
            {
                for(int j = 0; j < state.Cells.GetLength(1); j++)
                {
                    if(state.Cells[i, j] == Faction.None) continue;
                    if(state.Cells[i, j] != player) continue;

                    foreach(var direction in directions)
                    {
                        int captureAmount = GetPossibleDirectionalCaptureAmount(player, new Vector2Int(i, j), direction, state);
                        if(captureAmount == 0) continue;

                        var targetCell = new Vector2Int(i, j) + direction * (captureAmount + 1); // +1 because the target cell is the one after the captured tokens
                        if(legalMoves.Contains((targetCell.x, targetCell.y))) continue;

                        legalMoves.Add((targetCell.x, targetCell.y));
                    }
                }
            }

            return legalMoves.ToArray();
        }

        /// <summary>
        /// Returns all the coordinates of the outflanked tokens that need to be flipped.
        /// </summary>
        /// <param name="state">Current Board State</param>
        /// <returns></returns>
        public (int xCor, int yCor)[] GetAllOutflankedTokens(BoardState state)
        {
            var capturedTokens = new List<(int, int)>();

            Vector2Int anchorPoint = state.LastPlacedDiscCoordinates.ToVector2Int();
            Faction player = state.Cells[anchorPoint.x, anchorPoint.y];

            foreach (var direction in directions)
            {
                int captureAmount = GetTrueDirectionalCaptureAmount(player, anchorPoint, direction, state);

                if (captureAmount == 0) continue;

                for (int i = 1; i <= captureAmount; i++)
                {
                    var captured = anchorPoint + direction * i;
                    capturedTokens.Add((captured.x, captured.y));
                }
            }

            Debug.Log($"Outflanked: {capturedTokens.Count}");

            return capturedTokens.ToArray();
        }

        private int GetPossibleDirectionalCaptureAmount(Faction player, Vector2Int anchorPoint, Vector2Int direction, BoardState state)
        {
            return GetDirectionalOutflankAmount(player, anchorPoint, direction, state, isOutflanking: false);
        }

        private int GetTrueDirectionalCaptureAmount(Faction player, Vector2Int anchorPoint, Vector2Int direction, BoardState state)
        {
            return GetDirectionalOutflankAmount(player, anchorPoint, direction, state, isOutflanking: true);
        }

        private int GetDirectionalOutflankAmount(Faction player, Vector2Int anchorPoint, Vector2Int direction, BoardState state, bool isOutflanking)
        {
            if(!IsFirstNeigbourOpponent()) return 0;

            anchorPoint += direction;
            var outflankAmount = 1; // Start with 1 because the first neighbour is already an opponent's token

            while(true)
            {
                switch(GetDirectionalNeighbour(anchorPoint, direction, state))
                {
                    case Faction.OutOfBounds:
                        return 0;

                    case Faction.None:
                        return isOutflanking ? 0 : outflankAmount;

                    case Faction.Black:
                        if(player == Faction.Black)
                            return isOutflanking ? outflankAmount : 0;
                        break;

                    case Faction.White:
                        if(player == Faction.White)
                            return isOutflanking ? outflankAmount : 0;
                        break;
                }

                outflankAmount++;
                anchorPoint += direction;
            }

            bool IsFirstNeigbourOpponent()
            {
                var neighbour = GetDirectionalNeighbour(anchorPoint, direction, state);
                return neighbour != Faction.None && neighbour != Faction.OutOfBounds && neighbour != player;
            }
        }

        private Faction GetDirectionalNeighbour(Vector2Int anchorPoint, Vector2Int direction, BoardState state)
        {
            anchorPoint += direction;

            bool isOutOfBounds = anchorPoint.x < 0 || anchorPoint.x >= state.Cells.GetLength(0) ||
                                 anchorPoint.y < 0 || anchorPoint.y >= state.Cells.GetLength(1);

            if(isOutOfBounds) return Faction.OutOfBounds;

            return state.Cells[anchorPoint.x, anchorPoint.y];
        }
    }
}