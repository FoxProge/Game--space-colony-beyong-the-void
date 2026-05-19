namespace Space_colony_game.World.Buildings
{
    public abstract class Entity
    {
        public int Id { get; init; }
        public string Name { get; init; } = "";
        public int Col { get; init; }
        public int Row { get; init; }

        public abstract void OnDayPassed();
        public abstract void Draw(Systems.Camera camera);
    }
}
