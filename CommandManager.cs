// CommandManager.cs
using System.Collections.Generic;

namespace BoardGameFramework.Commands
{
    // Manages the execution, undoing, and redoing of commands.
    // It implements the Command Pattern by storing executed commands in dedicated stacks.
    
    public class CommandManager
    {
        private readonly Stack<ICommand> _undoStack = new(); // Executes a new command and pushes it onto the undo stack.
        private readonly Stack<ICommand> _redoStack = new();   // Clears the redo stack because redo history becomes invalid after a new action is performed.
        public void Execute(ICommand command)
        {
            command.Execute(); // Perform the action
            _undoStack.Push(command); // Add to undo history
            _redoStack.Clear(); // Clear redo history when there is a new action
        }

        public bool CanUndo => _undoStack.Count > 0; // There must be at least one command to undo
        public bool CanRedo => _redoStack.Count > 0;// There must be at least one command to redo

        public void Undo()  // Undo the last command
        {
            if (!CanUndo)
                return;

            var cmd = _undoStack.Pop(); // Get the last executed command
            cmd.Undo();   // Revert the action
            _redoStack.Push(cmd); // Add it to the redo stack so it can be redone if needed
        }
        // Redoes the most recently undone command
        public void Redo()
        {
            if (!CanRedo)
                return;

            var cmd = _redoStack.Pop();
            cmd.Execute();
            _undoStack.Push(cmd);
        }
        // Clears all command history. Used when loading a game or starting a new one.
        public void Clear()
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }
    // Exposes undo/redo history for save/load functionality.
       
        public IEnumerable<ICommand> GetUndoHistory() => _undoStack;
        public IEnumerable<ICommand> GetRedoHistory() => _redoStack;
    }
}
