using BoardGameFramework.Core;
using BoardGameFramework.Players;
using BoardGameFramework.UI;

public abstract class Player
{
    public string Name { get; }
    public string Token { get; }

    protected Player(string name, string token)
    {
        Name = name;
        Token = token;
    }

    // Change the return type from a Tuple to the Move class
    public abstract Move GetMove(IBoard board, IDisplay display, List<int> availableNumbers);
}