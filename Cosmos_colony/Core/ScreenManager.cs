using Space_colony_game.Screens;

namespace Space_colony_game.Core
{
    public class ScreenManager
    {
        private IScreen? current_;
        private IScreen? next_;
        public void GoTo(IScreen screen) => next_ = screen;

        public void Update()
        {
            if(next_ != null)
            {
                current_?.OnExit();
                current_ = next_;
                next_ = null;
                current_.OnEnter();
            }

            current_?.Update();
        }

        public void Draw() => current_?.Draw();
    }
}
