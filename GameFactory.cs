using System;
using BoardGameFramework.Core;
using BoardGameFramework.Players;
using BoardGameFramework.UI;
using BoardGameFramework.Games.Gomoku;
using BoardGameFramework.Games.NTTT;
using BoardGameFramework.Games.Notakto;
using BoardGameFramework.AI;

namespace BoardGameFramework
{
    public class GameFactory
    {
        // Remove the static _strategy field so we can pick a specific one per game
        public Game CreateGame(string gameType, IDisplay display)
        {
            return gameType switch
            {
                "NumericalTicTacToeGame" => new NumericalTicTacToeGame(display),
                "NotaktoGame" => new NotaktoGame(display),
                "GomokuGame" => new GomokuGame(display),
                _ => throw new ArgumentException($"Unknown game type: {gameType}")
            };
        }

        public Game CreateNewGame(string gameType, string mode, IDisplay display, string p1Name, string p2Name)
        {
            var game = CreateGame(gameType, display);
            // Pass the names into the configuration logic
            ConfigurePlayers(game, gameType, mode, p1Name, p2Name);
            game.CurrentPlayer = game.Players[0];
            return game;
        }

        private void ConfigurePlayers(Game game, string gameType, string mode, string p1Name, string p2Name)
        {
            game.Players.Clear();

            IComputerStrategy strategy = gameType == "NumericalTicTacToeGame"
                ? new NumericalTicTacToeStrategy()
                : new RandomStrategy();

            switch (mode)
            {
                case "HvH":
                    game.Players.Add(new HumanPlayer(p1Name, "X"));
                    game.Players.Add(new HumanPlayer(p2Name, "O"));
                    break;
                case "HvC":
                    game.Players.Add(new HumanPlayer(p1Name, "X"));
                    game.Players.Add(new ComputerPlayer("Computer", "O", strategy));
                    break;
                case "CvH":
                    game.Players.Add(new ComputerPlayer("Computer", "X", strategy));
                    game.Players.Add(new HumanPlayer(p2Name, "O"));
                    break;
            }
        }
    }
}