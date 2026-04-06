using BoardGameFramework.Commands;
using BoardGameFramework.Core;
using BoardGameFramework.Players;
using BoardGameFramework.UI;
using System;
using System.Collections.Generic;

namespace BoardGameFramework.Games.Gomoku
{
    public class GomokuGame : Game
    {
        public GomokuGame(IDisplay display) : base(display) { }

        protected override void SetupGame()
        {
            // Assuming your GomokuBoard constructor is standard
            Board = new GomokuBoard();
        }

        protected override void TakeTurn(Player player)
        {
            bool turnCompleted = false;

            while (!turnCompleted)
            {
                // 1. Create a dummy list for compatibility with the updated Player.GetMove signature
                var dummyList = new List<int>();

                // 2. Get the move from the player
                var moveResult = player.GetMove(Board, Display, dummyList);

                // 3. HANDLE META-COMMANDS (u, s, r, h, q)
                // We check if this is a NumericalMove (used for signals) or if your 
                // standard Move class has a way to flag commands.
                if (moveResult is NumericalMove navMove && navMove.Value == -1)
                {
                    string cmd = ((char)navMove.Row).ToString().ToLower();

                    bool isUndoRedo = (cmd == "u" || cmd == "r");
                    HandleMetaCommands(cmd);

                    if (isUndoRedo)
                    {
                        // Signal the main loop to restart the turn for the new CurrentPlayer
                        throw new OperationCanceledException("Turn aborted due to Undo/Redo");
                    }

                    Display.ShowBoard(Board);
                    continue;
                }

                // 4. VALIDATE AND EXECUTE
                // Use CAPITAL Row and Col to match the Move class properties
                if (IsValidMove(moveResult))
                {
                    ExecuteMove(moveResult.Row, moveResult.Col, player.Token);
                    turnCompleted = true;

                    // Artificial delay for Computer players
                    if (player is ComputerPlayer)
                    {
                        System.Threading.Thread.Sleep(1000);
                    }
                }
                else
                {
                    Display.ShowMessage("Invalid move. Position already occupied or out of bounds.");
                }
            }
        }

        private bool IsValidMove(Move move)
        {
            return move.Row >= 0 && move.Row < Board.Rows &&
                   move.Col >= 0 && move.Col < Board.Cols &&
                   string.IsNullOrEmpty(Board.GetCellValue(move.Row, move.Col));
        }

        private void ExecuteMove(int r, int c, string token)
        {
            var cmd = new MoveCommand(Board, r, c, token);
            CommandManager.Execute(cmd);

            LastMoveRow = r;
            LastMoveCol = c;
            LastMoveToken = token;
        }

        protected override bool CheckWin() =>
            Board.CheckWin(LastMoveRow, LastMoveCol, LastMoveToken);

        protected override bool IsDraw() => Board.IsFull();

        protected override void EndOfGame() => Display.ShowMessage("Game Over.");
    }
}