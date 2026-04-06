using BoardGameFramework.Core;
using BoardGameFramework.Games.Notakto; // Make sure to include this!
using System;
using System.Collections.Generic;
using System.Linq;

namespace BoardGameFramework.AI
{
    public class RandomStrategy : IComputerStrategy
    {
        private readonly Random _random = new Random();

        public (int row, int col, int value) SelectMove(IBoard board, string token, List<int> availableNumbers)
        {
            // 1. SPECIAL CASE: NOTAKTO
            if (board is NotaktoBoard nb)
            {
                // Find all boards that aren't "dead" yet
                var aliveBoards = nb.Boards
                    .Select((b, index) => new { SubBoard = b, Index = index })
                    .Where(x => !x.SubBoard.CheckWin(0, 0, "X"))
                    .ToList();

                if (aliveBoards.Any())
                {
                    // Pick a random alive board
                    var chosen = aliveBoards[_random.Next(aliveBoards.Count)];

                    // Find all empty cells on THAT specific sub-board
                    var subBoardEmptyCells = new List<(int r, int c)>();
                    for (int r = 0; r < chosen.SubBoard.Rows; r++)
                    {
                        for (int c = 0; c < chosen.SubBoard.Cols; c++)
                        {
                            if (string.IsNullOrEmpty(chosen.SubBoard.GetCellValue(r, c)))
                                subBoardEmptyCells.Add((r, c));
                        }
                    }

                    if (subBoardEmptyCells.Any())
                    {
                        var (r, c) = subBoardEmptyCells[_random.Next(subBoardEmptyCells.Count)];
                        // VALUE = Board Number (1, 2, or 3)
                        return (r, c, chosen.Index + 1);
                    }
                }
            }

            // 2. STANDARD CASE: SINGLE BOARD (Tic-Tac-Toe, Numerical, Gomoku)
            var emptyCells = new List<(int r, int c)>();
            for (int r = 0; r < board.Rows; r++)
            {
                for (int c = 0; c < board.Cols; c++)
                {
                    if (string.IsNullOrEmpty(board.GetCellValue(r, c)))
                        emptyCells.Add((r, c));
                }
            }

            if (emptyCells.Count == 0) return (-1, -1, -1);

            var (row, col) = emptyCells[_random.Next(emptyCells.Count)];

            int val = (availableNumbers != null && availableNumbers.Count > 0)
                      ? availableNumbers[_random.Next(availableNumbers.Count)]
                      : 1;

            return (row, col, val);
        }
    }
}