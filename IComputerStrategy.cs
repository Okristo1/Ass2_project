// ===============================
// IComputerStrategy.cs
// ===============================
using BoardGameFramework.Core;

namespace BoardGameFramework.AI
{
    public interface IComputerStrategy
    {
        // Added 'availableNumbers' list to the signature
        (int row, int col, int value) SelectMove(IBoard board, string token, List<int> availableNumbers);
    }
}