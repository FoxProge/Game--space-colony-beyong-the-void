using Raylib_cs;
using Space_colony_game.Screens;


namespace Space_colony_game.Core
{
    /// <summary>
    /// Главный класс игры.
    /// Отвечает за инициализацию окна, игровой цикл и завершение приложения.
    /// Управляет ScreenManager и глобальными настройками.
    /// </summary>
    public class Game
    {
        /// <summary>Ширина окна игры в пикселях.</summary>
        public static int ScreenWidth { get; private set; } = 1280;

        /// <summary>Высота окна игры в пикселях.</summary>
        public static int ScreenHeight { get; private set; } = 720;

        /// <summary>Целевой FPS игры.</summary>
        public const int TargetFPS = 60;

        /// <summary>Заголовок окна игры.</summary>
        public const string Title = "Space colony Beyond the Void";

        /// <summary>Флаг выхода из игры (используется для принудительного завершения цикла).</summary>
        public static bool ShouldExit { get; set; } = false;

        private readonly ScreenManager screenManager_ = new();

        /// <summary>
        /// Коэффициент масштабирования относительно базового разрешения 720p.
        /// Используется для адаптивного UI.
        /// </summary>
        private static float Scale_ => ScreenHeight / 720f;

        /// <summary>
        /// Масштабирует float значение под текущее разрешение экрана.
        /// </summary>
        /// <param name="value">Базовое значение.</param>
        /// <returns>Масштабированное значение.</returns>
        public static float ScaleF(float value) => value * Scale_;

        /// <summary>
        /// Масштабирует значение и приводит к int (для пикселей UI).
        /// </summary>
        /// <param name="value">Базовое значение.</param>
        /// <returns>Масштабированное целочисленное значение.</returns>
        public static int ScaleInt(float value) => (int)ScaleF(value);

        /// <summary>
        /// Запускает игру: инициализация -> игровой цикл -> завершение.
        /// </summary>
        public void Run()
        {
            Init();
            Loop();
            Shutdown();
        }

        /// <summary>
        /// Инициализация окна, настроек, ассетов и стартовой сцены.
        /// </summary>
        private void Init()
        {
            GameSettings.Load();
            if (!GameSettings.Current.Fullscreen)
            {
                ScreenWidth = GameSettings.Current.ScreenWidth;
                ScreenHeight = GameSettings.Current.ScreenHeight;
                if (ScreenWidth == 1920)
                    ScreenHeight -= 51;
                Raylib.InitWindow(ScreenWidth, ScreenHeight, Title);
            }
            else
            {
                Raylib.SetConfigFlags(ConfigFlags.BorderlessWindowMode);
                Raylib.InitWindow(1280, 720, Title);
                int monitor = Raylib.GetCurrentMonitor();
                ScreenWidth = Raylib.GetMonitorWidth(monitor);
                ScreenHeight = Raylib.GetMonitorHeight(monitor) - 51;
                Raylib.SetWindowSize(ScreenWidth, ScreenHeight);
                Raylib.SetWindowPosition(0, 0);
                ScreenWidth = Raylib.GetScreenWidth();
                ScreenHeight = Raylib.GetScreenHeight();
            }

            Raylib.SetTargetFPS(TargetFPS);
            Assets.Assets.Load();
            screenManager_.GoTo(new MainMenuScreen(screenManager_));
        }

        /// <summary>
        /// Основной игровой цикл.
        /// Обрабатывает обновление и отрисовку текущего экрана.
        /// </summary>
        private void Loop()
        {
            while(!Raylib.WindowShouldClose() && !ShouldExit)
            {
                screenManager_.Update();

                Raylib.BeginDrawing();
                Raylib.ClearBackground(Assets.Assets.BackgroundScreenColor);

                screenManager_.Draw();
                Raylib.EndDrawing();
            }
        }

        /// <summary>
        /// Завершение работы игры: сохранение данных, выгрузка ресурсов, закрытие окна.
        /// </summary>
        private void Shutdown()
        {
            GameSettings.Save();
            Assets.Assets.Unload();
            Raylib.CloseWindow();
        }
    }
}
