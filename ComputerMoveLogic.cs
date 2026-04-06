// ===============================
// ComputerMoveLogic.cs
// ===============================
using System;
using System.Collections.Generic;
using BoardGameFramework.Core;

namespace BoardGameFramework.AI
{
    public class ComputerMoveLogic : IComputerStrategy
    {
        private readonly Random _rnd = new Random();

        public (int row, int col, int value) SelectMove(IBoard board, string token, List<int> availableNumbers)
        {
            if (availableNumbers.Count == 0)
                throw new InvalidOperationException("No numbers left to play!");

            // 1. Try to find a winning move using ANY of the available numbers
            foreach (int val in availableNumbers)
            {
                var winningPos = FindWinningMove(board, token, val);
                if (winningPos != null)
                {
                    return (winningPos.Value.row, winningPos.Value.col, val);
                }
            }

            // 2. No immediate win? Pick a random position and a random available number
            var validPositions = GetAllValidMoves(board);
            var randomPos = validPositions[_rnd.Next(validPositions.Count)];
            var randomVal = availableNumbers[_rnd.Next(availableNumbers.Count)];

            return (randomPos.row, randomPos.col, randomVal);
        }

        private (int row, int col)? FindWinningMove(IBoard board, string token, int value)
        {
            var validMoves = GetAllValidMoves(board);
            string valString = value.ToString();

            foreach (var (row, col) in validMoves)
            {
                // Place the move
                board.PlaceMove(row, col, valString);

                bool wins = board.CheckWin(row, col, valString);

                // Undo the move
                board.ClearCell(row, col);

                if (wins)
                    return (row, col);
            }

            return null;
        }

        private List<(int row, int col)> GetAllValidMoves(IBoard board)
        {
            var moves = new List<(int row, int col)>();

            for (int r = 0; r < board.Rows; r++)
            {
                for (int c = 0; c < board.Cols; c++)
                {
                    if (board.IsCellEmpty(r, c))
                        moves.Add((r, c));
                }
            }

            return moves;
        }
    }
}
