using BoardGameFramework.Core;
using System.Collections.Generic;
using System.Linq;

namespace BoardGameFramework.Games.Notakto
{
    public class NotaktoBoard : BaseBoard
    {
        // This matches the 'boards' property in your UML diagram
        public List<IBoard> Boards { get; private set; }

        // Constructor for NEW games (3 boards, each 3x3)
        public NotaktoBoard() : base(3, 3)
        {
            InitializeBoards(3, 3);
        }

        // Constructor for LOADED games
        public NotaktoBoard(int rows, int cols) : base(rows, cols)
        {
            InitializeBoards(rows, cols);
        }

        private void InitializeBoards(int rows, int cols)
        {
            // Creates board1, board2, and board3 as seen in your diagram
            Boards = new List<IBoard>
            {
                new Board(rows, cols),
                new Board(rows, cols),
                new Board(rows, cols)
            };
        }

        // Since NotaktoBoard is now a container, we override PlaceMove 
        // to handle which board (1, 2, or 3) is being played on.
        public override void PlaceMove(int row, int col, string value)
        {
            // For a quick test, let's send everything to the first board in the list
            if (Boards.Count > 0)
            {
                // Notakto always uses "X" regardless of the 'value' passed
                Boards[0].PlaceMove(row, col, "X");
            }
        }
        // Inside NotaktoBoard.cs
        public override string? GetCellValue(int row, int col)
        {
            // Redirect requests to the first sub-board in our list
            if (Boards != null && Boards.Count > 0)
            {
                return Boards[0].GetCellValue(row, col);
            }
            return null;
        }
        // We also need to override GetCellValue to return null for the container grid
        // so the game doesn't get confused between the main container and the sub-boards.
        //   public override string? GetCellValue(int row, int col) => null;
        public override bool CheckWin(int row, int col, string token)
        {
            // Notakto logic: You LOSE if you complete a 3-in-a-row.
            // The game is only over when ALL boards have a 3-in-a-row.
            return Boards.All(b => b.CheckWin(0, 0, "X"));
        }

        public override bool IsFull()
        {
            // The game is a draw/full only when all sub-boards are full
            return Boards.All(b => b.IsFull());
        }
        
    }
}