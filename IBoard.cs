
namespace BoardGameFramework.Core
{
    public interface IBoard
    {
        int Rows { get; }
        int Cols { get; }
        int value { get; set; } 

        bool IsValidMove(int row, int col);
        void PlaceMove(int row, int col, string value);
        void ClearCell(int row, int col);

        bool IsCellEmpty(int row, int col);
        string? GetCellValue(int row, int col);

        bool CheckWin(int row, int col, string token);
        bool IsFull();
    }
}
