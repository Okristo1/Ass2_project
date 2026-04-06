using BoardGameFramework.AI;
using BoardGameFramework.Core;
using BoardGameFramework.Games.NTTT;
using BoardGameFramework.Games.Notakto;
using BoardGameFramework.Games.Gomoku;
using BoardGameFramework.Players;
using BoardGameFramework.UI;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace BoardGameFramework.Persistence
{
    public class HistoryManager
    {
        private readonly GameFactory _factory = new GameFactory();

        private class SaveData
        {
            public string GameType { get; set; }
            public int Rows { get; set; }
            public int Cols { get; set; }
            // NEW: Support for multiple boards (like your Notakto diagram)
            public List<string?[,]> Grids { get; set; }
            public List<PlayerInfo> Players { get; set; }
            public int CurrentPlayerIndex { get; set; }
            public List<MoveRecord> MoveHistory { get; set; }
        }

        private class PlayerInfo
        {
            public string Name { get; set; }
            public bool IsComputer { get; set; }
            public string Token { get; set; }
        }

        private class MoveRecord
        {
            public int Row { get; set; }
            public int Col { get; set; }
            public string Value { get; set; }
        }

        public void Save(Game game, string filePath)
        {
            var data = new SaveData
            {
                GameType = game.GetType().Name,
                Rows = game.Board.Rows,
                Cols = game.Board.Cols,
                Grids = ExtractAllGrids(game.Board), // Logic updated for multiple boards
                Players = ExtractPlayers(game),
                CurrentPlayerIndex = game.Players.IndexOf(game.CurrentPlayer),
                MoveHistory = ExtractMoveHistory(game)
            };

            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public Game Load(string filePath, IDisplay display)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Save file not found.");

            var json = File.ReadAllText(filePath);
            var data = JsonConvert.DeserializeObject<SaveData>(json);

            if (data == null) throw new Exception("Failed to load save file.");

            var game = _factory.CreateGame(data.GameType, display);

            // Provision the specific board container
            if (data.GameType == "NumericalTicTacToeGame")
                game.InitialiseLoadedBoard(new NumericalTicTacToeBoard(data.Rows));
            else if (data.GameType == "NotaktoGame")
                game.InitialiseLoadedBoard(new NotaktoBoard(data.Rows, data.Cols));
            else if (data.GameType == "GomokuGame")
                game.InitialiseLoadedBoard(new GomokuBoard(data.Rows, data.Cols));

            // Restore the state
            RestoreAllBoards(game.Board, data);
            RestorePlayers(game, data);
            RestoreMoveHistory(game, data);

            return game;
        }

        // --- UPDATED HELPERS FOR MULTI-BOARD SUPPORT ---

        private List<string?[,]> ExtractAllGrids(IBoard board)
        {
            var allGrids = new List<string?[,]>();

            // Check if this is your specialized NotaktoBoard container from the diagram
            if (board is NotaktoBoard multiBoard && multiBoard.Boards != null)
            {
                foreach (var b in multiBoard.Boards)
                {
                    allGrids.Add(GetGridData(b));
                }
            }
            else
            {
                // Standard single board (Numerical TTT or Gomoku)
                allGrids.Add(GetGridData(board));
            }
            return allGrids;
        }

        private string?[,] GetGridData(IBoard board)
        {
            var grid = new string?[board.Rows, board.Cols];
            for (int r = 0; r < board.Rows; r++)
                for (int c = 0; c < board.Cols; c++)
                    grid[r, c] = board.GetCellValue(r, c);
            return grid;
        }

        private void RestoreAllBoards(IBoard board, SaveData data)
        {
            if (board == null || data.Grids == null) return;

            if (board is NotaktoBoard multiBoard && multiBoard.Boards != null)
            {
                // Restore each board in the Notakto list (board1, board2, board3)
                for (int i = 0; i < Math.Min(multiBoard.Boards.Count, data.Grids.Count); i++)
                {
                    FillGrid(multiBoard.Boards[i], data.Grids[i]);
                }
            }
            else
            {
                // Restore single board
                FillGrid(board, data.Grids[0]);
            }
        }

        private void FillGrid(IBoard board, string?[,] gridData)
        {
            for (int r = 0; r < board.Rows; r++)
                for (int c = 0; c < board.Cols; c++)
                    if (gridData[r, c] != null)
                        board.PlaceMove(r, c, gridData[r, c]);
        }

        // --- REMAINDER OF HELPERS ---

        private List<PlayerInfo> ExtractPlayers(Game game)
        {
            var list = new List<PlayerInfo>();
            foreach (var p in game.Players)
            {
                list.Add(new PlayerInfo { Name = p.Name, IsComputer = p is ComputerPlayer, Token = p.Token });
            }
            return list;
        }

        private void RestorePlayers(Game game, SaveData data)
        {
            var tempPlayers = new List<Player>();
            foreach (var info in data.Players)
            {
                IComputerStrategy strategy = data.GameType switch
                {
                    "NumericalTicTacToeGame" => new NumericalTicTacToeStrategy(),
                    _ => new RandomStrategy()
                };

                Player p = info.IsComputer
                    ? new ComputerPlayer(info.Name, info.Token, strategy)
                    : new HumanPlayer(info.Name, info.Token);
                tempPlayers.Add(p);
            }
            game.LoadPlayers(tempPlayers, tempPlayers[data.CurrentPlayerIndex]);
        }

        private List<MoveRecord> ExtractMoveHistory(Game game) => new List<MoveRecord>();
        private void RestoreMoveHistory(Game game, SaveData data) { }
    }
}