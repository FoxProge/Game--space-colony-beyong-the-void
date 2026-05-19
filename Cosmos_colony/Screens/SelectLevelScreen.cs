using Raylib_cs;
using System.Numerics;
using Space_colony_game.Core;
using Space_colony_game.UI;

namespace Space_colony_game.Screens
{
    public class SelectLevelScreen : IScreen
    {
        private readonly ScreenManager screenManager_;
        private readonly Button btnBack_ = new(
            Game.ScreenWidth / 2 - 340,
            Game.ScreenHeight - Game.ScaleInt(110),
            180, 50, "Назад"
        );
        private readonly Button btnStart_ = new(
            Game.ScreenWidth / 2 + 160,
            Game.ScreenHeight - Game.ScaleInt(110),
            180, 50, "Начать"
        );
        private readonly LevelCard level_1_ = new(
            Game.ScreenWidth / 2 - 340,
            Game.ScaleInt(200), 300, 360,
            Assets.Assets.DefaultButtonColor,
            "Марс",
            "\nНормальная сложность"
        );
        private readonly LevelCard level_2_ = new(
            Game.ScreenWidth / 2 + 40,
            Game.ScaleInt(200), 300, 360,
            Assets.Assets.DefaultButtonColor,
            "Титан",
            "\nПовышенная сложность"
        );

        public SelectLevelScreen(ScreenManager screenManager)
        {
            screenManager_ = screenManager;
        }

        public void OnEnter() { }

        public void OnExit() { }

        public void Update()
        {
            if (!Raylib.IsMouseButtonPressed(MouseButton.Left)) return;

            if(level_1_.IsPressed())
            {
                level_1_.IsSelected = true;
                level_2_.IsSelected = false;
            }
            
            if(level_2_.IsPressed())
            {
                level_1_.IsSelected = false;
                level_2_.IsSelected = true;
            }

            if (btnBack_.IsLMB_Pressed())
                screenManager_.GoTo(new MainMenuScreen(screenManager_));
            if (btnStart_.IsLMB_Pressed())
            {
                if (level_1_.IsSelected)
                    screenManager_.GoTo(new LevelOne(screenManager_, 0));
                else if (level_2_.IsSelected) 
                    screenManager_.GoTo(new LevelTwo(screenManager_, 2));
            }
        }

        public void Draw()
        {
            DrawBgImage();
            DrawTitle();
            level_1_.Draw();
            level_2_.Draw();
            DrawPlanetsIcons();
            btnBack_.Draw();
            btnStart_.Draw();
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
            const string text = "Выбор уровня";
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

        private void DrawPlanetsIcons()
        {
            float imgSize = 200f;
            float fImgLevel_1_X = level_1_.Body.X + (level_1_.Body.Width - imgSize) / 2f;
            float fImgLevel_1_Y = level_1_.Body.Y + 40f;
            int imgLevel_1_X = (int)MathF.Round(fImgLevel_1_X);
            int imgLevel_1_Y = (int)MathF.Round(fImgLevel_1_Y);

            float fImgLevel_2_X = level_2_.Body.X + (level_2_.Body.Width - imgSize) / 2f;
            float fImgLevel_2_Y = level_2_.Body.Y + 40f;
            int imgLevel_2_X = (int)MathF.Round(fImgLevel_2_X);
            int imgLevel_2_Y = (int)MathF.Round(fImgLevel_2_Y);

            if (Assets.Assets.PlanetMars.HasValue)
                Raylib.DrawTexturePro(
                    Assets.Assets.PlanetMars.Value,
                    new Rectangle(
                        0, 0,
                        Assets.Assets.PlanetMars.Value.Width,
                        Assets.Assets.PlanetMars.Value.Height
                    ),
                    new Rectangle(imgLevel_1_X, imgLevel_1_Y, imgSize, imgSize),
                    Vector2.Zero, 0f, Color.White
                );
            
            if (Assets.Assets.PlanetTitan.HasValue)
                Raylib.DrawTexturePro(
                    Assets.Assets.PlanetTitan.Value,
                    new Rectangle(
                        0, 0,
                        Assets.Assets.PlanetTitan.Value.Width,
                        Assets.Assets.PlanetTitan.Value.Height
                    ),
                    new Rectangle(imgLevel_2_X, imgLevel_2_Y, imgSize, imgSize),
                    Vector2.Zero, 0f, Color.White
                );
        }
    }
}
