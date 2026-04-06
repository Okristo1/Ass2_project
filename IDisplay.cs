// ===============================
// IDisplay.cs
// ===============================
using BoardGameFramework.Core;

namespace BoardGameFramework.UI
{
    public interface IDisplay
    {
        void ShowMessage(string message);
        string GetInput(string prompt);
        void ShowBoard(IBoard board);
        void ShowHelp();
        void Clear();   // Recommended for usability
    }
}
