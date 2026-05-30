using Raylib_cs;
using System.Numerics;
using Space_colony_game.Core;
using Space_colony_game.UI;


namespace Space_colony_game.Screens
{
    /// <summary>
    /// Главное меню игры: отрисовка заголовка, кнопок перехода и обработка базового ввода.
    /// </summary>
    public class MainMenuScreen : IScreen
    {
        /// <summary>Менеджер экранов для навигации между экранами.</summary>
        private readonly ScreenManager screenManager_;

        /// <summary>Опциональная фонова́я текстура (если используется).</summary>
        private Texture2D BbImage;

        /// <summary>Кнопка запуска игры / выбора уровня.</summary>
        private readonly Button btnPlay_ = new(
            Game.ScaleInt(490), Game.ScaleInt(364),
            Game.ScaleInt(300), Game.ScaleInt(54),
            "Играть"
        );

        /// <summary>Кнопка перехода к настройкам.</summary>
        private readonly Button btnSettings_ = new(
            Game.ScaleInt(490), Game.ScaleInt(438),
            Game.ScaleInt(300), Game.ScaleInt(54),
            "Настройки"
        );

        /// <summary>Кнопка выхода из игры.</summary>
        private readonly Button btnExit_ = new(
            Game.ScaleInt(490), Game.ScaleInt(512),
            Game.ScaleInt(300), Game.ScaleInt(54),
            "Выход"
        );

        /// <summary>
        /// Создаёт экран главного меню.
        /// </summary>
        /// <param name="screenManager">Менеджер экранов, используемый для переходов.</param>
        public MainMenuScreen(ScreenManager screenManager)
        {
            screenManager_ = screenManager;
        }

        /// <summary>Вызывается при активации экрана (здесь не используется).</summary>
        public void OnEnter() { }

        /// <summary>Вызывается при деактивации экрана (здесь не используется).</summary>
        public void OnExit() { }

        /// <summary>
        /// Обрабатывает ввод: при нажатии ЛКМ проверяет состояние кнопок и выполняет переход/выход.
        /// </summary>
        public void Update()
        {
            if (!Raylib.IsMouseButtonPressed(MouseButton.Left)) return;

            if (btnPlay_.IsAvailable && btnPlay_.IsHovered())
                screenManager_.GoTo(new SelectLevelScreen(screenManager_));
            else if (btnSettings_.IsAvailable && btnSettings_.IsHovered()) 
                screenManager_.GoTo(new SettingsScreen(screenManager_));
            else if (btnExit_.IsAvailable && btnExit_.IsHovered())
                Game.ShouldExit = true;
        }

        /// <summary>Рисует меню: фон, заголовок, кнопки и версию приложения.</summary>
        public void Draw()
        {
            DrawBgImage();
            DrawTitle();
            btnPlay_.Draw();
            btnSettings_.Draw();
            btnExit_.Draw(exitBtn: true);    
            DrawVersion();
        }

        /// <summary>
        /// Рисует фоновую текстуру экрана, если она загружена в <see cref="Assets.Assets.BackgroundScreenImage"/>.
        /// </summary>
        public void DrawBgImage()
        {
            if (Assets.Assets.BackgroundScreenImage == null) return;

            Raylib.DrawTexturePro(
                Assets.Assets.BackgroundScreenImage.Value,
                new Rectangle(
                    0, 0,
                    Assets.Assets.BackgroundScreenImage.Value.Width,
                    Assets.Assets.BackgroundScreenImage.Value.Height
                ),
                new Rectangle(0, 0, Game.ScreenWidth, Game.ScreenHeight),
                Vector2.Zero, 0f,
                new Color(180, 180, 180, 255)
            );
        }

        /// <summary>Рисует заголовок игры по центру экрана.</summary>
        private static void DrawTitle()
        {
            const string title = "Space colony: Beyond the Void";

            Vector2 size = Raylib.MeasureTextEx(
                Assets.Assets.FontLarge, title,
                Assets.Assets.FontLargeSize, 1
            );

            float fx = (Game.ScreenWidth - size.X) / 2f;
            int x = (int)MathF.Round(fx);
            int y = 160;

            Raylib.DrawTextEx(
                Assets.Assets.FontLarge, title,
                new Vector2(x, y),
                Assets.Assets.FontLargeSize, 1, Color.White
            );
        }

        /// <summary>Рисует версию игры в правом нижнем углу экрана.</summary>
        private static void DrawVersion()
        {
            const string ver = "v0.2";

            Vector2 size = Raylib.MeasureTextEx(
                Assets.Assets.FontMedium, ver, 16, 1
            );

            float fx = Game.ScreenWidth - size.X - 12;
            float fy = Game.ScreenHeight - size.Y - 10;

            int x = (int)MathF.Round(fx);
            int y = (int)MathF.Round(fy);

            Raylib.DrawTextEx(
                Assets.Assets.FontMedium, ver,
                new Vector2(x, y),
                16, 1, Color.Green
            );
        }
    }
}
