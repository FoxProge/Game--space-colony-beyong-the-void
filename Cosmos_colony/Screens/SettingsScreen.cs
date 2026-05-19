using Raylib_cs;
using Space_colony_game.Core;
using Space_colony_game.UI;
using System.Numerics;

namespace Space_colony_game.Screens
{
    public class SettingsScreen : IScreen
    {
        private readonly ScreenManager screenManager_;
        private readonly (int W, int H)[] Resolutions =
        {
            (1280, 720),
            (1600, 900),
            (1920, 1080)
        };

        private int resolutionIdx_;
        private int originalResolutionIdx_;
        private bool fullscreen_;
        private bool originalFullscreen_;
        private bool SettingsChanged =>
            resolutionIdx_ != originalResolutionIdx_ ||
            fullscreen_ != originalFullscreen_;

        private Rectangle SettingsPanel;
        private readonly Button btnBack;
        private readonly Button btnSave;
        private readonly Button btnResolutionOne;
        private readonly Button btnResolutionTwo;
        private readonly Button btnResolutionThree;
        private readonly Button btnFullscreenOn;
        private readonly Button btnFullscreenOff;

        public SettingsScreen(ScreenManager screenManager)
        {
            screenManager_ = screenManager;
            SettingsPanel = new(
                (Game.ScreenWidth - Game.ScaleInt(500)) / 2f,
                (Game.ScreenHeight - Game.ScaleInt(280)) / 2f - Game.ScaleF(40),
                Game.ScaleInt(500), Game.ScaleInt(280)
            );
            btnBack = new(
                Game.ScreenWidth / 2f - Game.ScaleInt(190),
                SettingsPanel.Y + SettingsPanel.Height + Game.ScaleInt(20),
                Game.ScaleInt(180), Game.ScaleInt(44),
                "Назад"
            );
            btnSave = new(
                Game.ScreenWidth / 2f + Game.ScaleInt(10),
                SettingsPanel.Y + SettingsPanel.Height + Game.ScaleInt(20),
                Game.ScaleInt(180), Game.ScaleInt(44),
                "Сохранить"
            );
            btnResolutionOne = new(
                SettingsPanel.X + Game.ScaleInt(20),
                SettingsPanel.Y + Game.ScaleInt(50),
                Game.ScaleInt(130), Game.ScaleInt(36),
                "1280x720"
            );
            btnResolutionTwo = new(
                btnResolutionOne.Body.X + Game.ScaleInt(140),
                SettingsPanel.Y + Game.ScaleInt(50),
                Game.ScaleInt(130), Game.ScaleInt(36),
                "1600x900"
            );
            btnResolutionThree = new(
                btnResolutionTwo.Body.X + Game.ScaleInt(140),
                SettingsPanel.Y + Game.ScaleInt(50),
                Game.ScaleInt(130), Game.ScaleInt(36),
                "1920x1080"
            );
            btnFullscreenOn = new(
                SettingsPanel.X + Game.ScaleInt(20),
                SettingsPanel.Y + Game.ScaleInt(148),
                Game.ScaleInt(100), Game.ScaleInt(36),
                "Вкл"
            );
            btnFullscreenOff = new(
                btnFullscreenOn.Body.X + Game.ScaleInt(100),
                SettingsPanel.Y + Game.ScaleInt(148),
                Game.ScaleInt(100), Game.ScaleInt(36),
                "Выкл"
            );
        }

        public void OnEnter()
        {
            resolutionIdx_ = Array.FindIndex(Resolutions,
                r => r.W == GameSettings.Current.ScreenWidth &&
                    r.H == GameSettings.Current.ScreenHeight
            );

            if(resolutionIdx_ == -1) resolutionIdx_ = 0;

            fullscreen_ = GameSettings.Current.Fullscreen;
            originalResolutionIdx_ = resolutionIdx_;
            originalFullscreen_ = fullscreen_;
        }

        public void OnExit() { }

        public void Update()
        {
            if (btnBack.IsLMB_Pressed())
                screenManager_.GoTo(new MainMenuScreen(screenManager_));
            else if (btnSave.IsLMB_Pressed())
            {
                if (SettingsChanged)
                {
                    GameSettings.Current.ScreenWidth = Resolutions[resolutionIdx_].W;
                    GameSettings.Current.ScreenHeight = Resolutions[resolutionIdx_].H;
                    GameSettings.Current.Fullscreen = fullscreen_;
                    GameSettings.Save();
                }
                screenManager_.GoTo(new MainMenuScreen(screenManager_));
            }
            else if (btnResolutionOne.IsLMB_Pressed())
                resolutionIdx_ = 0;

            else if (btnResolutionTwo.IsLMB_Pressed())
                resolutionIdx_ = 1;

            else if (btnResolutionThree.IsLMB_Pressed())
                resolutionIdx_ = 2;

            else if (btnFullscreenOn.IsLMB_Pressed())
                fullscreen_ = true;

            else if (btnFullscreenOff.IsLMB_Pressed())
                fullscreen_ = false;
        }

        public void Draw()
        {
            DrawBgImage();
            DrawTitle();
            DrawPanel();
            SetButtonColors();
            btnResolutionOne.Draw();
            btnResolutionTwo.Draw();
            btnResolutionThree.Draw();
            btnFullscreenOn.Draw();
            btnFullscreenOff.Draw();
            DrawMessage();
            btnBack.Draw();
            btnSave.Draw();
        }

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
                new Color(120, 120, 120, 255)
            );
        }

        private void DrawTitle()
        {
            const string text = "Настройки";
            Vector2 textSize = Raylib.MeasureTextEx(Assets.Assets.FontLarge, text, 42, 1);
            float fTextX = (Game.ScreenWidth - textSize.X) / 2f;
            int textX = (int)MathF.Round(fTextX);
            int textY = 100;

            Raylib.DrawTextEx(
                Assets.Assets.FontLarge, text,
                new Vector2(textX, textY), 42,
                1, Color.White
            );
        }

        private void DrawPanel()
        {
            Raylib.DrawRectangleRec(
                SettingsPanel,
                Assets.Assets.BackgroundScreenColor
            );
            Raylib.DrawRectangleLinesEx(
                SettingsPanel, 1.5f,
                Assets.Assets.DefaultBorderColor
            );

            Raylib.DrawTextEx(
                Assets.Assets.FontMedium, "Разрешение экрана",
                new Vector2(
                    SettingsPanel.X + Game.ScaleInt(20),
                    SettingsPanel.Y + Game.ScaleInt(20)),
                Assets.Assets.FontMediumSize, 1, new Color(200, 212, 224, 255));

            float divY = SettingsPanel.Y + Game.ScaleInt(100);
            Raylib.DrawLineV(
                new Vector2(SettingsPanel.X + Game.ScaleInt(10), divY),
                new Vector2(SettingsPanel.X + SettingsPanel.Width - Game.ScaleInt(10), divY),
                Assets.Assets.DefaultBorderColor);

            Raylib.DrawTextEx(
                Assets.Assets.FontMedium, "Полноэкранный режим",
                new Vector2(
                    SettingsPanel.X + Game.ScaleInt(20),
                    SettingsPanel.Y + Game.ScaleInt(118)),
                Assets.Assets.FontMediumSize, 1, new Color(200, 212, 224, 255));
        }

        private void DrawMessage()
        {
            if (!SettingsChanged) return;

            const string msg = "* Настройки вступят в силу при перезагрузке игры";

            Vector2 msgSize = Raylib.MeasureTextEx(
                Assets.Assets.FontSmall, msg,
                Assets.Assets.FontSmallSize, 1);

            float msgX = SettingsPanel.X + Game.ScaleInt(20);
            float msgY = SettingsPanel.Y + SettingsPanel.Height - Game.ScaleInt(36);

            Raylib.DrawRectangleRec(
                new Rectangle(
                    msgX - Game.ScaleInt(4),
                    msgY - Game.ScaleInt(4),
                    msgSize.X + Game.ScaleInt(8),
                    msgSize.Y + Game.ScaleInt(8)),
                new Color(10, 16, 24, 180)
            );

            Raylib.DrawTextEx(
                Assets.Assets.FontSmall, msg,
                new Vector2(msgX, msgY),
                Assets.Assets.FontSmallSize, 1,
                Assets.Assets.SelectionColor
            );
        }

        private void SetButtonColors()
        {
           btnResolutionOne.BackgroundColor = resolutionIdx_ == 0
                ? Assets.Assets.DefaultButtonHoveredColor
                : Assets.Assets.DefaultButtonColor;

            btnResolutionTwo.BackgroundColor = resolutionIdx_ == 1
                ? Assets.Assets.DefaultButtonHoveredColor
                : Assets.Assets.DefaultButtonColor;

            btnResolutionThree.BackgroundColor = resolutionIdx_ == 2
                ? Assets.Assets.DefaultButtonHoveredColor
                : Assets.Assets.DefaultButtonColor;

            btnFullscreenOn.BackgroundColor = fullscreen_
                ? Assets.Assets.DefaultButtonHoveredColor
                : Assets.Assets.DefaultButtonColor;

            btnFullscreenOff.BackgroundColor = !fullscreen_
                ? Assets.Assets.DefaultButtonHoveredColor
                : Assets.Assets.DefaultButtonColor;
        }
    }
}
