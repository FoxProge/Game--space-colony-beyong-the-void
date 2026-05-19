using Space_colony_game.Core;

namespace Space_colony_game
{
    public class Program
    {
        private static Game? game;
        static void Main(string[] args)
        {
            game = new Game();
            game.Run();
        }
    }
}