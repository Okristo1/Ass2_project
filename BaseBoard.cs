using BoardGameFramework.Core;

public abstract class BaseBoard : IBoard
{
    protected string?[,] grid;

    public int Rows { get; protected set; }
    public int Cols { get; protected set; }
    public int value { get; set; }  // For games that need a numeric value (e.g., Numerical TTT)

    protected BaseBoard(int rows, int cols)
    {
        Rows = rows;
        Cols = cols;
        grid = new string?[rows, cols];
    }

    public abstract bool CheckWin(int row, int col, string token);

    public virtual bool IsValidMove(int row, int col)
    {
        if (row < 0 || col < 0 || row >= Rows || col >= Cols)
            return false;

        return grid[row, col] == null;
    }

    public virtual void PlaceMove(int row, int col, string value)
    {
        grid[row, col] = value;
    }

    public virtual void ClearCell(int row, int col)
    {
        grid[row, col] = null;
    }

    public bool IsCellEmpty(int row, int col) => grid[row, col] == null;

    public virtual string? GetCellValue(int row, int col) => grid[row, col]; //  

    // Inside BaseBoard.cs
    public virtual bool IsFull()
    {
        //logic that checks if the single grid is full
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++)
            {
                if (string.IsNullOrEmpty(grid[r, c])) return false;
            }
        }
        return true;
    }
}
