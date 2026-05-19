using Raylib_cs;
using Space_colony_game.Screens;


namespace Space_colony_game.Core
{
    public class Game
    {
        public static int ScreenWidth { get; private set; } = 1280;
        public static int ScreenHeight { get; private set; } = 720;

        public const int TargetFPS = 60;
        public const string Title = "Space colony Beyond the Void";

        public static bool ShouldExit { get; set; } = false;

        private readonly ScreenManager screenManager_ = new();

        private static float Scale_ => ScreenHeight / 720f;
        public static float ScaleF(float value) => value * Scale_;
        public static int ScaleInt(float value) => (int)ScaleF(value);
        public static float ScaleFontSmallSize => Assets.Assets.FontSmallSize * ScaleF(1);
        public static float ScaleFontMediumSize => Assets.Assets.FontMediumSize * ScaleF(1);
        public static float ScaleFontLargeSize => Assets.Assets.FontLargeSize * ScaleF(1);

        public void Run()
        {
            Init();
            Loop();
            Shutdown();
        }

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

        private void Shutdown()
        {
            GameSettings.Save();
            Assets.Assets.Unload();
            Raylib.CloseWindow();
        }
    }
}
