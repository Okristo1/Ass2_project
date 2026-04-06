using BoardGameFramework.UI;

namespace BoardGameFramework
{
    public static class Program
    {
        public static void Main()
        {
            // 1. Create the UI (ConsoleDisplay)
            IDisplay display = new ConsoleDisplay();

            // 2. Initialize the Controller with that UI
            var controller = new GameController(display);

            // 3. Let the Controller take over the loop, menus, and factory logic
            controller.Run();
        }
    }
}