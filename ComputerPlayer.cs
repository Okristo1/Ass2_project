// ComputerPlayer.cs
using BoardGameFramework.AI;
using BoardGameFramework.Core;
using BoardGameFramework.UI;
using System.Collections.Generic;
using BoardGameFramework.Players;

namespace BoardGameFramework.Players
{
    // Represents an AI-controlled player that delegates its move-making logic to an interchangeable strategy (The Strategy Design Pattern)
 
    public class ComputerPlayer : Player
    {
        // Encapsulated strategy
        private readonly IComputerStrategy _strategy;

        // Initializes a new ComputerPlayer with a specific AI strategy.
       
        // param name="name" ,The name of the AI (e.g., "AI Bot)
        // param name="token", The token assigned to this player (e.g., "X" or "O").
        // param name="strategy", The logic used to calculate the best move.
        public ComputerPlayer(string name, string token, IComputerStrategy strategy) : base(name, token)
        {
            _strategy = strategy;
        }
        // Executes the AI's turn by requesting a move from the injected strategy
        public override Move GetMove(IBoard board, IDisplay display, List<int> availableNumbers)
        {
            // Delegate the decision-making process to the strategy object
            var moveData = _strategy.SelectMove(board, Token, availableNumbers);

            // Display the AI's choice to the user. 
   
            display.ShowMessage($"{Name} plays {moveData.value} at ({moveData.row + 1}, {moveData.col + 1}).");

            // Returns a NumericalMove to maintain framework compatibility across NTTT, Notakto, and Gomoku
            return new NumericalMove(moveData.row, moveData.col, moveData.value, this.Name);
        }
    }
}