// ===============================
// HumanPlayer.cs
// ===============================
using BoardGameFramework.Core;
using BoardGameFramework.UI;
using BoardGameFramework.Games.Notakto; // Required for the type check
using System.Collections.Generic;
using System;

namespace BoardGameFramework.Players
{
    public class HumanPlayer : Player
    {
        public HumanPlayer(string name, string token) : base(name, token)
        {
        }

        public override Move GetMove(IBoard board, IDisplay display, List<int> availableNumbers)
        {
            while (true)
            {
                // 1. CONTEXT-AWARE PROMPT
                // If it's Notakto, we prompt for Board first. Otherwise, Row/Col first.
                if (board is NotaktoBoard)
                {
                    display.ShowMessage($"{Name}, enter move as 'board row col' (e.g., 1 2 2 for Board 1, center):");
                }
                else
                {
                    display.ShowMessage($"{Name}, enter move as 'row col number' (or 'u' undo, 's' save, 'h' help, 'r' redo):");
                }

                var input = display.GetInput("> ").ToLower().Trim();

                if (string.IsNullOrWhiteSpace(input)) continue;

                // 2. CHECK FOR META-COMMANDS
                if (input == "u" || input == "s" || input == "h" || input == "q" || input == "r")
                {
                    return new NumericalMove((int)input[0], 0, -1, this.Name);
                }

                var parts = input.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

                // 3. VALIDATE 3-PART INPUT
                if (parts.Length != 3 ||
                    !int.TryParse(parts[0], out int p1) ||
                    !int.TryParse(parts[1], out int p2) ||
                    !int.TryParse(parts[2], out int p3))
                {
                    display.ShowMessage("Invalid input. Please enter three numbers or a command letter.");
                    continue;
                }

                int row, col, val;

                // 4. LOGICAL MAPPING BASED ON GAME TYPE
                if (board is NotaktoBoard)
                {
                    // For Notakto: Input is [Board] [Row] [Col]
                    val = p1;        // The Board Number (1, 2, or 3)
                    row = p2 - 1;    // Convert to 0-based
                    col = p3 - 1;    // Convert to 0-based
                }
                else
                {
                    // For Numerical: Input is [Row] [Col] [Number]
                    row = p1 - 1;
                    col = p2 - 1;
                    val = p3;
                }

                // 5. BOUNDARY CHECK
                // For Notakto, we check boundaries against a sub-board (they are all the same size)
                // For others, we check against the main board.
                var checkBoard = (board is NotaktoBoard nb) ? nb.Boards[0] : board;

                if (row < 0 || row >= checkBoard.Rows || col < 0 || col >= checkBoard.Cols)
                {
                    display.ShowMessage("That position is out of bounds. Try again.");
                    continue;
                }

                // 6. RETURN THE MOVE
                return new NumericalMove(row, col, val, this.Name);
            }
        }
    }
}