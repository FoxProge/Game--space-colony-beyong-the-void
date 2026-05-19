using Raylib_cs;

namespace Space_colony_game.UI.Panels
{
    public abstract class Panel
    {
        public abstract Rectangle Body { get; set; }
        public abstract Color BackgroundColor { get; set; }
        public abstract Color BorderColor { get; set; }
        public abstract float BorderThickness { get; set; }
        public abstract void Draw();
    }
}
