
namespace Space_colony_game.World
{
    public class Resource
    {
        public string Name { get; init; } = "";
        public float Value { get; set; } = 0;
        public float Max { get; set; } = 200;
        public float Delta { get; set; } = 0;
        public float CriticalLevel { get; set; } = 20;
        public bool IsCritical => Value <= CriticalLevel;
    }
}
