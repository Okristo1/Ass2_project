namespace BoardGameFramework.Players
{
    public abstract class Move
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public int Value { get; set; }
        // NEW: Track who made the move so Undo can return the number to the right hand
        public string PlayerName { get; set; }
    }

    public class NumericalMove : Move
    {
        public NumericalMove(int row, int col, int value, string playerName)
        {
            this.Row = row;
            this.Col = col;
            this.Value = value;
            this.PlayerName = playerName;
        }
    }
}