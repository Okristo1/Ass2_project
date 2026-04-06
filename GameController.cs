
// GameController.cs
// ===============================
using System;
using BoardGameFramework.UI;
using BoardGameFramework.Persistence;

namespace BoardGameFramework
{
    public class GameController
    {
        private readonly IDisplay _display;
        private readonly GameFactory _factory;

        public GameController(IDisplay display)
        {
            _display = display;
            _factory = new GameFactory();
        }

        public void Run()
        {
            while (true)
            {
                _display.ShowMessage("=== Main Menu ===");
                _display.ShowMessage("1. Numerical Tic-Tac-Toe");
                _display.ShowMessage("2. Notakto");
                _display.ShowMessage("3. Gomoku");
                _display.ShowMessage("4. Load Game");
                _display.ShowMessage("5. Help");
                _display.ShowMessage("6. Exit");

                string choice = _display.GetInput("Select an option: ");

                switch (choice)
                {
                    case "1":
                        StartGame("NumericalTicTacToeGame");
                        break;

                    case "2":
                        StartGame("NotaktoGame");
                        break;

                    case "3":
                        StartGame("GomokuGame");
                        break;

                    case "4":
                        LoadGame();
                        break;

                    case "5":
                        _display.ShowHelp();
                        break;

                    case "6":
                        _display.ShowMessage("Goodbye!");
                        return;

                    default:
                        _display.ShowMessage("Invalid choice.");
                        break;
                }
            }
        }

        private void StartGame(string gameType)
        {
            _display.ShowMessage("Select mode:");
            _display.ShowMessage("1. Human vs Human\n2. Human vs Computer\n3. Computer vs Human");
            string modeChoice = _display.GetInput("Mode: ");

            // Prompt for names
            string p1Name = "Player 1";
            string p2Name = "Player 2";

            if (modeChoice == "1") // HvH
            {
                p1Name = _display.GetInput("Enter Name for Player 1: ");
                p2Name = _display.GetInput("Enter Name for Player 2: ");
            }
            else if (modeChoice == "2") // HvC
            {
                p1Name = _display.GetInput("Enter your name: ");
            }
            else if (modeChoice == "3") // CvH
            {
                p2Name = _display.GetInput("Enter your name: ");
            }

            string mode = modeChoice switch { "1" => "HvH", "2" => "HvC", "3" => "CvH", _ => "HvH" };

            // Pass the names to the factory
            var game = _factory.CreateNewGame(gameType, mode, _display, p1Name, p2Name);
            game.PlayGame();
        }

        private void LoadGame()
        {
            string filename = _display.GetInput("Enter save filename: ");
            try
            {
                var saveManager = new GameSaver(new HistoryManager());
                var game = saveManager.Load(filename, _display);
                game.PlayGame();
            }
            catch (Exception ex)
            {
                _display.ShowMessage($"Failed to load game: {ex.Message}");
            }
        }
    }
}
/*using System;
using BoardGameFramework.UI;
using BoardGameFramework.Persistence;
using BoardGameFramework.Commands; // Ensure this is included

namespace BoardGameFramework
{
    public class GameController
    {
        private readonly IDisplay _display;
        private readonly GameFactory _factory;
        private readonly HistoryManager _historyManager; // Persist history management

        public GameController(IDisplay display)
        {
            _display = display;
            _factory = new GameFactory();
            _historyManager = new HistoryManager(); // Initialize once
        }

        public void Run()
        {
            // ... (Your existing while(true) menu logic is perfect) ...
        }

        private void StartGame(string gameType)
        {
            _display.ShowMessage("Select mode:");
            _display.ShowMessage("1. Human vs Human");
            _display.ShowMessage("2. Human vs Computer");
            _display.ShowMessage("3. Computer vs Human");

            string modeChoice = _display.GetInput("Mode: ");

            string mode = modeChoice switch
            {
                "1" => "HvH",
                "2" => "HvC",
                "3" => "CvH",
                _ => "HvH"
            };

            try 
            {
                var game = _factory.CreateNewGame(gameType, mode, _display);
                
                // IMPORTANT: If your Game class has a CommandManager, 
                // clear it here so old history from a previous session doesn't leak in.
                game.CommandManager.Clear(); 

                game.PlayGame();
            }
            catch (Exception ex)
            {
                _display.ShowMessage($"Error starting game: {ex.Message}");
            }
        }

        private void LoadGame()
        {
            string filename = _display.GetInput("Enter save filename: ");
            try
            {
                // Use the persistent _historyManager instead of a local one
                var game = _historyManager.Load(filename, _display);
                
                _display.ShowMessage("Game loaded successfully!");
                game.PlayGame();
            }
            catch (Exception ex)
            {
                _display.ShowMessage($"Failed to load game: {ex.Message}");
            }
        }
    }
}*/