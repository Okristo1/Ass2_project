// ===============================
// Game.cs (abstract)
// Template Method Pattern
// ===============================
using System;
using System.Collections.Generic;
using BoardGameFramework.Commands;
using BoardGameFramework.Persistence;
using BoardGameFramework.Players;
using BoardGameFramework.UI;

namespace BoardGameFramework.Core
{
    // Abstract base class implementing the Template Method Pattern.
    // Each concrete game defines its own setup, turn-taking logic,   win conditions, and end-of-game behaviour.
    
    public abstract class Game
    {
        // ------------------------------
        // Properties
        // ------------------------------
        public IBoard Board { get; protected set; }                 // Board abstraction for all games
        public List<Player> Players { get; private set; }           // Supports any number of players
        public Player CurrentPlayer { get; set; }                    // Tracks whose turn it is

        protected CommandManager CommandManager;                    // Undo/Redo system
        protected IDisplay Display;                                 // UI abstraction
        protected GameSaver GameSaver;                              // Save/Load functionality
        protected bool IsGameOver;

        // Tracks the last move for win-checking
        protected int LastMoveRow { get; set; }
        protected int LastMoveCol { get; set; }
        protected string LastMoveToken { get; set; } = string.Empty;

        // ------------------------------
        // Constructor
        // ------------------------------
        protected Game(IDisplay display)
        {
            Display = display;
            CommandManager = new CommandManager();
            GameSaver = new GameSaver(new HistoryManager());
            Players = new List<Player>();
        }

        // ------------------------------
        // BOARD INITIALISATION
        // ------------------------------
        public void InitialiseLoadedBoard(IBoard loadedBoard)
        {
            this.Board = loadedBoard;
        }

        // ===============================
        // TEMPLATE METHOD
        // ===============================
        public void PlayGame()
        {
            if (Board == null)
            {
                SetupGame();
            }

            IsGameOver = false;

            if (Players.Count > 0 && CurrentPlayer == null)
                CurrentPlayer = Players[0];

            while (!IsGameOver)
            {
                Display.ShowBoard(Board);

                try
                {
                    TakeTurn(CurrentPlayer);

                    if (CheckWin())
                    {
                        Display.ShowBoard(Board);
                        Display.ShowMessage($"{CurrentPlayer.Name} wins!");
                        IsGameOver = true;
                    }
                    else if (IsDraw())
                    {
                        Display.ShowBoard(Board);
                        Display.ShowMessage("It's a draw.");
                        IsGameOver = true;
                    }
                    else
                    {
                        SwitchPlayer();
                    }
                }
                catch (OperationCanceledException)
                {
                    // If the user triggered an exit (via Save -> Quit or 'Q' directly)
                    if (IsGameOver)
                    {
                        return; // Exit PlayGame immediately to return to Main Menu
                    }

                    // Otherwise, it was a standard Undo/Redo reset
                    Display.ShowMessage("Turn reset following Undo/Redo.");
                }
            }

            EndOfGame();
        }

        // ===============================
        // ABSTRACT METHODS (Template Hooks)
        // ===============================
        protected abstract void SetupGame();
        protected abstract void TakeTurn(Player player);
        protected abstract bool CheckWin();
        protected abstract bool IsDraw();
        protected abstract void EndOfGame();

        // ===============================
        // PLAYER MANAGEMENT
        // ===============================
        protected void SwitchPlayer()
        {
            if (Players.Count == 0 || CurrentPlayer == null)
                return;

            int index = Players.IndexOf(CurrentPlayer);
            CurrentPlayer = Players[(index + 1) % Players.Count];
        }

        public void LoadPlayers(List<Player> players, Player current)
        {
            this.Players.Clear();
            this.Players.AddRange(players);
            this.CurrentPlayer = current;
        }

        private void DecrementPlayer()
        {
            int index = Players.IndexOf(CurrentPlayer);
            int prevIndex = (index - 1 + Players.Count) % Players.Count;
            CurrentPlayer = Players[prevIndex];
        }

        private bool IsComputerTurn() => CurrentPlayer is ComputerPlayer;

        // ===============================
        // UNDO / REDO
        // ===============================
        public void UndoMove()
        {
            if (CommandManager.CanUndo)
            {
                CommandManager.Undo();
                DecrementPlayer();

                if (IsComputerTurn() && CommandManager.CanUndo)
                {
                    CommandManager.Undo();
                    DecrementPlayer();
                }

                Display.ShowMessage("Move undone.");
            }
            else
            {
                Display.ShowMessage("No moves to undo.");
            }
        }

        public void RedoMove()
        {
            if (CommandManager.CanRedo)
            {
                CommandManager.Redo();
                SwitchPlayer();
                Display.ShowMessage("Move redone.");
            }
            else
            {
                Display.ShowMessage("No moves to redo.");
            }
        }

        // ===============================
        // SAVE / LOAD
        // ===============================
        public virtual void SaveGame(string filePath)
        {
            GameSaver.Save(this, filePath);
            Display.ShowMessage("Game saved successfully.");
        }

        public virtual void LoadGame(string filePath)
        {
            try
            {
                Game loadedState = GameSaver.Load(filePath, Display);

                if (loadedState != null)
                {
                    this.InitialiseLoadedBoard(loadedState.Board);
                    this.LoadPlayers(loadedState.Players, loadedState.CurrentPlayer);
                    Display.ShowMessage("Game loaded successfully.");
                }
            }
            catch (Exception ex)
            {
                Display.ShowMessage($"Failed to load game: {ex.Message}");
            }
        }

        // ===============================
        // META-COMMAND HANDLING
        // ===============================
        public virtual void ShowHelp()
        {
            Display.ShowHelp();
        }

        protected bool HandleMetaCommands(string input)
        {
            switch (input.ToLower())
            {
                case "u":
                    UndoMove();
                    return true;
                case "r":
                    RedoMove();
                    return true;
                case "s":
                    string savePath = Display.GetInput("Enter save filename: ");
                    SaveGame(savePath);
                    string choice = Display.GetInput("Game saved. (Q)uit or (C)ontinue? ").ToLower();
                    if (choice == "q")
                    {
                        IsGameOver = true;
                        throw new OperationCanceledException(); // Signal jump back to PlayGame catch
                    }
                    return true;
                case "q":
                    IsGameOver = true;
                    throw new OperationCanceledException(); // Signal jump back to PlayGame catch
                case "h":
                    ShowHelp();
                    return true;
                case "l":
                    string loadPath = Display.GetInput("Enter filename to load: ");
                    LoadGame(loadPath);
                    return true;
                default:
                    return false;
            }
        }
    }
}
