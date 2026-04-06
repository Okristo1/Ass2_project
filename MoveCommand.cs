 // ===============================
// MoveCommand.cs
// ===============================
using BoardGameFramework.Core;

namespace BoardGameFramework.Commands
{
    /// <summary>
    /// Encapsulates a single move on the board, including the
    /// row, column, and value placed. Implements ICommand so
    /// that moves can be undone and redone.
    /// </summary>
    public class MoveCommand : ICommand
    {
        private readonly IBoard _board;
        private readonly int _row;
        private readonly int _col;
        private readonly string _value;
        private readonly string _previousValue; // Store what was there before

        public MoveCommand(IBoard board, int row, int col, string value)
        {
            _board = board;
            _row = row;
            _col = col;
            _value = value;
            _previousValue = _board.GetCellValue(row, col); // Capture the previous value for undo purposes
        }

        public void Execute()
        {
            _board.PlaceMove(_row, _col, _value);
        }

        public void Undo()
        {
            _board.PlaceMove(_row, _col, _previousValue); // Restore the previous value (which could be null)
        }
    }
}
