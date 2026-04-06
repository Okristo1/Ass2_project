using BoardGameFramework.Core;

namespace BoardGameFramework.Core
{
    // This is a concrete class some boards needs (e.g NotaktoBoard)
    public class Board : BaseBoard
    {
     // The constructor that passes dimensions to the BaseBoard  to initialize the string?[,] grid
     public Board(int rows, int cols) : base(rows, cols)
        {
            

        }

        public override bool CheckWin(int row, int col, string token)
        {
            // Notakto-specific: Checks if the current move completed a 3-in-a-row.
            // In Notakto's misere play, completing a line 'kills' this sub-board.

            bool Line(params (int r, int c)[] cells)
            {
                foreach (var (r, c) in cells)
                {
                    if (r < 0 || r >= Rows || c < 0 || c >= Cols) return false;
                    if (GetCellValue(r, c) != "X") return false;
                }
                return true;
            }

            return
                Line((row, 0), (row, 1), (row, 2)) ||
                Line((0, col), (1, col), (2, col)) ||
                Line((0, 0), (1, 1), (2, 2)) ||
                Line((0, 2), (1, 1), (2, 0));
        }
    }
}