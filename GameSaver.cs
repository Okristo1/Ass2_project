// ===============================
// GameSaver.cs
// ===============================
using BoardGameFramework.Core;
using BoardGameFramework.UI;
using System;
namespace BoardGameFramework.Persistence
{
    public class GameSaver
    {
        private readonly HistoryManager _historyManager;

        public GameSaver(HistoryManager historyManager)
        {
            _historyManager = historyManager;
        }

        public void Save(Game game, string filePath)
        {
            try
            {
                _historyManager.Save(game, filePath);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save game: {ex.Message}");
            }
        }

        public Game Load (string filePath, IDisplay display)
        {
            try
            {
               return  _historyManager.Load(filePath, display);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load game: {ex.Message}");
            }
        }

    }
}
