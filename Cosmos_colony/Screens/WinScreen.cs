using Raylib_cs;
using Space_colony_game.Core;
using System.Numerics;

namespace Space_colony_game.Screens
{
    public class WinScreen : IScreen
    {
        private readonly ScreenManager screenManager_;
        private readonly int daysAlive_;

        public WinScreen(ScreenManager screenManager, int daysAlive)
        {
            screenManager_ = screenManager;
            daysAlive_ = daysAlive;
        }
        public void OnEnter() { }

        public void OnExit() { }

        public void Update()
        {
            if (Raylib.IsMouseButtonPressed(MouseButton.Left) ||
               Raylib.IsMouseButtonPressed(MouseButton.Right) ||
               Raylib.IsMouseButtonPressed(MouseButton.Middle) ||
               Raylib.GetKeyPressed() != 0)
            {
                screenManager_.GoTo(new MainMenuScreen(screenManager_));
            }

        }

        public void Draw()
        {
            DrawBgImage();
            DrawTitle();
            DrawStats();
            DrawHint();
        }

        public void DrawBgImage()
        {
            if (Assets.Assets.BackgroundScreenImage == null) return;

            Raylib.DrawTexturePro(
                Assets.Assets.BackgroundScreenImage.Value,
                new Rectangle(
                    0, 0,
                    Assets.Assets.BackgroundScreenImage.Value.Width,
                    Assets.Assets.BackgroundScreenImage.Value.Height),
                new Rectangle(0, 0, Game.ScreenWidth, Game.ScreenHeight),
                Vector2.Zero, 0f,
                new Color(120, 120, 120, 255));
        }

        private void DrawTitle()
        {
            const string text = "Вы победили!";
            Vector2 size = Raylib.MeasureTextEx(
                Assets.Assets.FontLarge, text, Assets.Assets.FontLargeSize, 1);

            Raylib.DrawTextEx(
                Assets.Assets.FontLarge, text,
                new Vector2(
                    MathF.Round((Game.ScreenWidth - size.X) / 2f),
                    MathF.Round(Game.ScreenHeight / 2f - size.Y - Game.ScaleInt(40))),
                Assets.Assets.FontLargeSize, 1,
                new Color(220, 180, 30, 255)); // золотой
        }

        private void DrawStats()
        {
            string text = $"Ваша колония продержалась: {daysAlive_} дней";
            Vector2 size = Raylib.MeasureTextEx(
                Assets.Assets.FontMedium, text, Assets.Assets.FontMediumSize, 1);

            Raylib.DrawTextEx(
                Assets.Assets.FontMedium, text,
                new Vector2(
                    MathF.Round((Game.ScreenWidth - size.X) / 2f),
                    MathF.Round(Game.ScreenHeight / 2f + Game.ScaleInt(10))),
                Assets.Assets.FontMediumSize, 1,
                new Color(200, 212, 224, 255));
        }

        private void DrawHint()
        {
            const string hint = "Нажмите любую кнопку чтобы вернуться в меню";
            Vector2 size = Raylib.MeasureTextEx(
                Assets.Assets.FontSmall, hint, Assets.Assets.FontSmallSize, 1);

            Raylib.DrawTextEx(
                Assets.Assets.FontSmall, hint,
                new Vector2(
                    MathF.Round((Game.ScreenWidth - size.X) / 2f),
                    MathF.Round(Game.ScreenHeight - Game.ScaleInt(80))),
                Assets.Assets.FontSmallSize, 1,
                new Color(100, 120, 140, 255));
        }

    }
}
