using System;
using BoardGameFramework.Core;
using BoardGameFramework.Games.Notakto; // Add this namespace!

namespace BoardGameFramework.UI
{
    public class ConsoleDisplay : IDisplay
    {
        public void ShowMessage(string message) => Console.WriteLine(message);

        public string GetInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine() ?? string.Empty;
        }

        public void Clear() => Console.Clear();

        public void ShowBoard(IBoard board)
        {
            //  Check if this is the multi-board Notakto container
            if (board is NotaktoBoard multiBoard && multiBoard.Boards != null)
            {
                // 2. Loop through each sub-board (board1, board2, board3)
                for (int i = 0; i < multiBoard.Boards.Count; i++)
                {
                    Console.WriteLine($"\n--- BOARD {i + 1} ---");


                    RenderSingleGrid(multiBoard.Boards[i]);
                }
            }
            else
            {
                // Normal single-board rendering (fallback)
                RenderSingleGrid(board);
            }
        }

        // 4. Move your existing rendering logic into this helper method
        private void RenderSingleGrid(IBoard board)
        {
            Console.WriteLine();

            // Column headers
            Console.Write("    ");
            for (int c = 0; c < board.Cols; c++)
                Console.Write($"{c + 1,3}");
            Console.WriteLine();

            Console.Write("    ");
            for (int c = 0; c < board.Cols; c++)
                Console.Write("---");
            Console.WriteLine();

            // Rows
            for (int r = 0; r < board.Rows; r++)
            {
                Console.Write($"{r + 1,3}|");

                for (int c = 0; c < board.Cols; c++)
                {
                    string symbol = board.GetCellValue(r, c) ?? ".";
                    Console.Write($"{symbol,3}");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public void ShowHelp()
        {
            // ... (keep your existing help text)
        }
    }
}
