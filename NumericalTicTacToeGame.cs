using System;
using System.Collections.Generic;
using System.Linq;
using BoardGameFramework.Commands;
using BoardGameFramework.Core;
using BoardGameFramework.Players;
using BoardGameFramework.UI;
using BoardGameFramework.Persistence;

namespace BoardGameFramework.Games.NTTT
{
    public class NumericalTicTacToeGame : Game
    {
        public NumericalTicTacToeGame(IDisplay display) : base(display) { }

        protected override void SetupGame()
        {
            Display.ShowMessage("--- Welcome to Numerical Tic-Tac-Toe ---");

            string choice = Display.GetInput("Type 'L' to Load a saved game, or press Enter for a New Game: ").ToLower();

            if (choice == "l")
            {
              // loads a saved game
                string loadPath = Display.GetInput("Enter filename to load (e.g., savegame.json): ");
                LoadGame(loadPath);
                return;
            }
            //takes user input to set up board size and validates the input
            Display.ShowMessage("--- Board Configuration ---");
            int size = 0;
            while (size < 3)
            {
                string input = Display.GetInput("Enter board size (3 or greater): ");
                if (!int.TryParse(input, out size) || size < 3)
                    Display.ShowMessage("Invalid input.");
            }
            Board = new NumericalTicTacToeBoard(size);
        }
        //Method to allow players to play during their turn
        protected override void TakeTurn(Player player)
        {
            bool turnCompleted = false;

            while (!turnCompleted)
            {
                // 1. Setup metadata for the UI
                int n = Board.Rows;
                int targetSum = (n * (n * n + 1)) / 2;
                List<int> available = GetLegalNumbers(player);

                if (player is HumanPlayer)
                {
                    Display.ShowMessage($"\n--- {player.Name.ToUpper()}'s TURN ({player.Token}) ---");
                    Display.ShowMessage($"Winning Target Sum: {targetSum}");
                    Display.ShowMessage($"Your Available Numbers: {string.Join(", ", available)}");
                }

                // 2. Get the move or command
                var moveResult = player.GetMove(Board, Display, available) as NumericalMove; 

                // 3. Handle Meta-Commands (u, r, s, h, q)
                if (moveResult.Value == -1)
                {
                    // Convert the stored row back to the command char (e.g., 'u', 's')
                    string cmd = ((char)moveResult.Row).ToString().ToLower();

                    bool isUndorRedo = (cmd == "u" || cmd == "r");

                    // This calls the base Game.cs implementation
                    HandleMetaCommands(cmd);

                    if (isUndorRedo)
                    {
                        // Signal the main PlayGame loop to restart the turn with the corrected player
                        throw new OperationCanceledException("Turn aborted due to Undo/Redo");
                    }

                    // For Save/Help, refresh board and loop again for the SAME player
                    Display.ShowBoard(Board);
                    continue;
                }

                // 4. Standard Move Validation & Execution
                if (ValidateAndExecute(moveResult, available))
                {
                    turnCompleted = true;
                }
            }
        }

        private bool ValidateAndExecute(NumericalMove move, List<int> available)
        {
            if (move.Row < 0 || move.Row >= Board.Rows || move.Col < 0 || move.Col >= Board.Cols)
            {
                Display.ShowMessage("Invalid position: Out of bounds.");
                return false;
            }

            if (!string.IsNullOrEmpty(Board.GetCellValue(move.Row, move.Col)))
            {
                Display.ShowMessage("That cell is already occupied.");
                return false;
            }

            if (!available.Contains(move.Value))
            {
                Display.ShowMessage($"The number {move.Value} is not legal for you.");
                return false;
            }

            ExecuteMove(move.Row, move.Col, move.Value);
            return true;
        }

        private void ExecuteMove(int r, int c, int val)
        {
            // Ensures that the MoveCommand logic handles putting the number back on Undo!
            var cmd = new MoveCommand(Board, r, c, val.ToString());
            CommandManager.Execute(cmd);

            LastMoveRow = r;
            LastMoveCol = c;
            LastMoveToken = val.ToString();
        }

        private List<int> GetLegalNumbers(Player player) //Ensure each player plays valid numbers
        {
            bool needsOdd = (player.Token == "X");
            int maxNumber = Board.Rows * Board.Cols;
            var allPossible = Enumerable.Range(1, maxNumber);

            var used = new HashSet<int>();
            for (int r = 0; r < Board.Rows; r++)
            {
                for (int c = 0; c < Board.Cols; c++)
                {
                    if (int.TryParse(Board.GetCellValue(r, c), out int n))
                        used.Add(n);
                }
            }

            return allPossible
                .Where(n => !used.Contains(n) && (n % 2 != 0) == needsOdd)
                .ToList();
        }

        protected override bool CheckWin() => Board.CheckWin(LastMoveRow, LastMoveCol, LastMoveToken);
        protected override bool IsDraw() => Board.IsFull();
        protected override void EndOfGame() => Display.ShowMessage("Game Over.");
    }
}