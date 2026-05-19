
namespace Space_colony_game.Screens
{
    public interface IScreen
    {
        void OnEnter();
        void OnExit();
        void Update();
        void Draw();
        void DrawBgImage();
    }
}
