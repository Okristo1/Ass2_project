// ===============================
// ICommand.cs
// ===============================
namespace BoardGameFramework.Commands
{
    /// <summary>
    /// Represents a reversible action in the Command Pattern.
    /// All game actions that support Undo/Redo must implement this interface.
    /// </summary>
    public interface ICommand
    {
        void Execute();
        void Undo();
    }
}

