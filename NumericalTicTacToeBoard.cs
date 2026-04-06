using BoardGameFramework.Core;
using System.Collections.Generic;

namespace BoardGameFramework.Games.NTTT
{
    public class NumericalTicTacToeBoard : BaseBoard
    {
        // Store the target sum as a field so it can be used in CheckWin
        private readonly int _targetSum;

        public NumericalTicTacToeBoard(int size) : base(size, size)
        {
            // Formula for the magic constant: n(n^2 + 1) / 2
            _targetSum = size * (size * size + 1) / 2;
        }

        // We override the abstract method from BaseBoard
        public override bool CheckWin(int row, int col, string token)
        {
            // Check win on rows
            for (int r = 0; r < Rows; r++)
                if (RowSum(r) == _targetSum) return true;

            // Checks win in Columns
            for (int c = 0; c < Cols; c++)
                if (ColSum(c) == _targetSum) return true;

            // Check win along Diagonals
            if (Diag1Sum() == _targetSum) return true;
            if (Diag2Sum() == _targetSum) return true;

            return false;
        }

        private int? RowSum(int r)
        {
            int sum = 0;
            for (int c = 0; c < Cols; c++)
            {
                string val = GetCellValue(r, c);
                if (string.IsNullOrEmpty(val)) return null;
                sum += int.Parse(val);
            }
            return sum;
        }

        private int? ColSum(int c)
        {
            int sum = 0;
            for (int r = 0; r < Rows; r++)
            {
                string val = GetCellValue(r, c);
                if (string.IsNullOrEmpty(val)) return null;
                sum += int.Parse(val);
            }
            return sum;
        }

        private int? Diag1Sum()
        {
            int sum = 0;
            for (int i = 0; i < Rows; i++)
            {
                string val = GetCellValue(i, i);
                if (string.IsNullOrEmpty(val)) return null;
                sum += int.Parse(val);
            }
            return sum;
        }

        private int? Diag2Sum()
        {
            int sum = 0;
            for (int i = 0; i < Rows; i++)
            {
                string val = GetCellValue(i, Rows - 1 - i);
                if (string.IsNullOrEmpty(val)) return null;
                sum += int.Parse(val);
            }
            return sum;
        }
    }
}