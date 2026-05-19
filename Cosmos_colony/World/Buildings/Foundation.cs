
namespace Space_colony_game.World.Buildings
{
    public class Foundation : Building
    {
        public override bool CanBeDestroyed => false;

        public Foundation() { }
        public override void OnDayPassed() { }

        public override void DrawBody(int x, int y, int w, int h)
            => DrawTextureOrFallback(Assets.Assets.TextureFoundation, x, y, w, h, Type.Color);
    }
}
