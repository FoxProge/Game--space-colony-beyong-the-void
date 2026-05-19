using Raylib_cs;
using Space_colony_game.Core;
using System.Numerics;

namespace Space_colony_game.Screens
{
    /// <summary>
    /// Экран поражения. Показывает заголовок, статистику (сколько дней держалась колония)
    /// и подсказку о возврате в главное меню по любому вводу.
    /// </summary>
    public class LoseScreen : IScreen
    {
        private readonly ScreenManager screenManager_;
        private readonly int daysAlive_;

        /// <summary>
        /// Создаёт экран поражения.
        /// </summary>
        /// <param name="screenManager">Менеджер экранов — используется для перехода в главное меню.</param>
        /// <param name="daysAlive">Число дней, которые продержалась колония до поражения.</param>
        public LoseScreen(ScreenManager screenManager, int daysAlive)
        {
            screenManager_ = screenManager;
            daysAlive_ = daysAlive;
        }

        /// <summary>Вызывается при входе на экран (здесь не используется).</summary>
        public void OnEnter() { }

        /// <summary>Вызывается при выходе с экрана (здесь не используется).</summary>
        public void OnExit() { }

        /// <summary>
        /// Обрабатывает ввод пользователя: любой клик мыши или нажатие клавиши
        /// приводит к возврату в главное меню.
        /// </summary>
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

        /// <summary>Рисует экран: фон, заголовок, статистику и подсказку.</summary>
        public void Draw()
        {
            DrawBgImage();
            DrawTitle();
            DrawStats();
            DrawHint();
        }

        /// <summary>
        /// Рисует фоновое изображение экрана, если оно загружено в <see cref="Assets.Assets.BackgroundScreenImage"/>.
        /// </summary>
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

        /// <summary>Рисует заголовок экрана поражения.</summary>
        private void DrawTitle()
        {
            const string text = "Вы проиграли!";
            Vector2 size = Raylib.MeasureTextEx(
                Assets.Assets.FontLarge, text, Assets.Assets.FontLargeSize, 1);

            Raylib.DrawTextEx(
                Assets.Assets.FontLarge, text,
                new Vector2(
                    MathF.Round((Game.ScreenWidth - size.X) / 2f),
                    MathF.Round(Game.ScreenHeight / 2f - size.Y - Game.ScaleInt(40))),
                Assets.Assets.FontLargeSize, 1,
                new Color(180, 40, 40, 255)); // Красный
        }

        /// <summary>Рисует статистику — сколько дней продержалась колония.</summary>
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

        /// <summary>Рисует подсказку внизу экрана о возврате в меню.</summary>
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
