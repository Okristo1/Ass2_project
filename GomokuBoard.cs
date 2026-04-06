using BoardGameFramework.Core;

namespace BoardGameFramework.Games.Gomoku
{
    public class GomokuBoard : BaseBoard
    {
        // 1. Default constructor for NEW games
        public GomokuBoard() : base(15, 15) { }

        // 2. Overloaded constructor for LOADED games (Required by HistoryManager)
        public GomokuBoard(int rows, int cols) : base(rows, cols) { }

        public override bool CheckWin(int row, int col, string token)
        {
            // Helper to count consecutive tokens in a specific direction
            int Count(int dr, int dc)
            {
                int r = row + dr;
                int c = col + dc;
                int count = 0;

                // Check bounds and token match
                while (r >= 0 && c >= 0 && r < Rows && c < Cols &&
                       GetCellValue(r, c) == token)
                {
                    count++;
                    r += dr;
                    c += dc;
                }
                return count;
            }

            // Check if placing a token at (row, col) creates a line of 5 or more
            bool Line(int dr, int dc)
            {
                // Current piece (1) + count in one direction + count in opposite direction
                return 1 + Count(dr, dc) + Count(-dr, -dc) >= 5;
            }

            // Check all 4 axes: Vertical, Horizontal, and both Diagonals
            return Line(1, 0) || // Vertical (|)
                   Line(0, 1) || // Horizontal (-)
                   Line(1, 1) || // Diagonal (\)
                   Line(1, -1);   // Diagonal (/)
        }
    }
}