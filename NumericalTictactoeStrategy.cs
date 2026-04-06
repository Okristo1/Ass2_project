using BoardGameFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BoardGameFramework.AI
{
    public class NumericalTicTacToeStrategy : IComputerStrategy
    {
        private readonly Random _random = new Random();

        public (int row, int col, int value) SelectMove(IBoard board, string token, List<int> availableNumbers)
        {
            var emptyCells = GetEmptyCells(board);

            // 1. CHECK FOR IMMEDIATE WIN
            foreach (var cell in emptyCells)
            {
                foreach (int num in availableNumbers)
                {
                    // Using the names 'row' and 'col' defined in the Tuple below
                    if (IsWinningMove(board, cell.row, cell.col, num))
                        return (cell.row, cell.col, num);
                }
            }

            // 2. CHECK FOR BLOCK (Stop the Human from winning)
            var opponentNumbers = GetOpponentNumbers(board, availableNumbers);
            foreach (var cell in emptyCells)
            {
                foreach (int oppNum in opponentNumbers)
                {
                    if (IsWinningMove(board, cell.row, cell.col, oppNum))
                    {
                        // Block using the smallest available valid number
                        return (cell.row, cell.col, availableNumbers[0]);
                    }
                }
            }

            // 3. NO WIN/BLOCK? Pick random
            var randomCell = emptyCells[_random.Next(emptyCells.Count)];
            int randomNum = availableNumbers[_random.Next(availableNumbers.Count)];
            return (randomCell.row, randomCell.col, randomNum);
        }

        private bool IsWinningMove(IBoard iBoard, int r, int c, int num)
        {
            // 1. Cast to BaseBoard (the class name from your snippet)
            if (iBoard is BaseBoard board)
            {
                // 2. Use GetCellValue to store the current state
                string? originalValue = board.GetCellValue(r, c);

                // 3. Use PlaceMove to simulate the move
                board.PlaceMove(r, c, num.ToString());

                // 4. Check if this results in a win
                bool win = board.CheckWin(r, c, num.ToString());

                // 5. Use ClearCell (or PlaceMove with the originalValue) to reset
                if (originalValue == null)
                {
                    board.ClearCell(r, c);
                }
                else
                {
                    board.PlaceMove(r, c, originalValue);
                }

                return win;
            }
            return false;
        }

        private List<int> GetOpponentNumbers(IBoard board, List<int> myNumbers)
        {
            // Determine the parity of the current AI player
            bool iAmOdd = myNumbers.Any(n => n % 2 != 0);

            // FIX: Use the actual board dimensions to determine the max possible number
            int totalCells = board.Rows * board.Cols;
            var allPossible = Enumerable.Range(1, totalCells);

            var used = new HashSet<int>();
            for (int r = 0; r < board.Rows; r++)
            {
                for (int c = 0; c < board.Cols; c++)
                {
                    if (int.TryParse(board.GetCellValue(r, c), out int n))
                        used.Add(n);
                }
            }

            // Return numbers that haven't been used and have the opposite parity of the AI
            return allPossible.Where(n => !used.Contains(n) && (n % 2 != 0) != iAmOdd).ToList();
        }
        private List<(int row, int col)> GetEmptyCells(IBoard board)
        {
            var cells = new List<(int row, int col)>(); // Explicitly naming the tuple fields
            for (int r = 0; r < board.Rows; r++)
            {
                for (int c = 0; c < board.Cols; c++)
                {
                    if (string.IsNullOrEmpty(board.GetCellValue(r, c)))
                        cells.Add((r, c));
                }
            }
            return cells;
        }
    }
}