using BoardGameFramework.Commands;
using BoardGameFramework.Core;
using BoardGameFramework.Players;
using BoardGameFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BoardGameFramework.Games.Notakto
{
    public class NotaktoGame : Game
    {
        public NotaktoGame(IDisplay display) : base(display) { }

        protected override void SetupGame()
        {
            Board = new NotaktoBoard();
        }

        protected override void TakeTurn(Player player)
        {
            bool turnCompleted = false;

            while (!turnCompleted)
            {
                var availableNumbers = new List<int> { 1 };
                var moveResult = player.GetMove(Board, Display, availableNumbers);

                // 1. HANDLE META-COMMANDS
                if (moveResult is NumericalMove navMove && navMove.Value == -1)
                {
                    string cmd = ((char)navMove.Row).ToString().ToLower();
                    bool isUndoRedo = (cmd == "u" || cmd == "r");
                    HandleMetaCommands(cmd);

                    if (isUndoRedo)
                    {
                        throw new OperationCanceledException("Turn aborted due to Undo/Redo");
                    }

                    Display.ShowBoard(Board);
                    continue;
                }

                // 2. PARSE AND VALIDATE
                var notaktoBoard = (NotaktoBoard)Board;
                int boardIdx = moveResult.Value - 1;
                int r = moveResult.Row;
                int c = moveResult.Col;

                if (IsValidNotaktoMove(notaktoBoard, boardIdx, r, c))
                {
                    ExecuteNotaktoMove(notaktoBoard, boardIdx, r, c);
                    turnCompleted = true;

                    if (player is ComputerPlayer)
                    {
                        System.Threading.Thread.Sleep(1000);
                    }
                }
            }
        }

        private bool IsValidNotaktoMove(NotaktoBoard nb, int bIdx, int r, int c)
        {
            if (bIdx < 0 || bIdx >= nb.Boards.Count)
            {
                Display.ShowMessage("Invalid Board! Choose 1, 2, or 3.");
                return false;
            }

            var targetBoard = nb.Boards[bIdx];

            // RULE: Block moves on dead boards
            // Passing 0,0 is fine here as NotaktoBoard.CheckWin scans the whole sub-grid
            if (targetBoard.CheckWin(0, 0, "X"))
            {
                Display.ShowMessage($"Board {bIdx + 1} is already dead! Choose a different board.");
                return false;
            }

            if (r < 0 || r >= targetBoard.Rows || c < 0 || c >= targetBoard.Cols ||
                !string.IsNullOrEmpty(targetBoard.GetCellValue(r, c)))
            {
                Display.ShowMessage("Invalid position. Cell is out of bounds or occupied.");
                return false;
            }

            return true;
        }

        private void ExecuteNotaktoMove(NotaktoBoard nb, int bIdx, int r, int c)
        {
            var targetBoard = nb.Boards[bIdx];
            var cmd = new MoveCommand(targetBoard, r, c, "X");
            CommandManager.Execute(cmd);

            LastMoveRow = r;
            LastMoveCol = c;
            LastMoveToken = "X";
        }

        // CRITICAL UPDATE: The game ends when ALL boards are "dead" (contain a 3-in-a-row)
        protected override bool CheckWin()
        {
            var nb = (NotaktoBoard)Board;
            // The game is over when every board in the list is "won" (dead)
            return nb.Boards.All(b => b.CheckWin(0, 0, "X"));
        }

        // In Notakto, a draw is technically impossible if played to completion, 
        // but we'll check if all cells are filled just in case.
        protected override bool IsDraw()
        {
            var nb = (NotaktoBoard)Board;
            return nb.Boards.All(b => b.IsFull()) && !CheckWin();
        }

        protected override void EndOfGame()
        {
            Display.ShowBoard(Board);
            // In Notakto, the player who makes the move that "kills" the final board LOSES.
            Display.ShowMessage($"\n*** {CurrentPlayer.Name} completed the final board and LOSES! ***");
            Display.ShowMessage("Game Over.");
        }
    }
}