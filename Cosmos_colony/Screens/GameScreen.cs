using Raylib_cs;
using Space_colony_game.Core;
using Space_colony_game.Systems;
using Space_colony_game.UI;
using Space_colony_game.UI.Panels;
using Space_colony_game.World;
using System.Numerics;

namespace Space_colony_game.Screens
{
    public abstract class GameScreen : IScreen
    {
        private const float MinimapScreenRatio = 0.22f;
        private float mapW => Game.ScreenWidth * MinimapScreenRatio;
        private float mapH => mapW * 0.66f;
        protected readonly ScreenManager screenManager_;
        protected static int Pad => Game.ScaleInt(12);
        protected static int PadIn => Game.ScaleInt(10);
        protected static int Gap => Game.ScaleInt(4);
        protected static int PanelWidth => Game.ScaleInt(500);
        protected static int PanelHeight => (int)(Game.ScreenHeight * 0.052f);
        public int levelIndex { get; private set; }
        protected readonly Colony colony_ = new();
        protected readonly DayTransition Transition = new();
        protected readonly WorldMap World_map;
        protected readonly MiniMap miniMap_;
        protected readonly Camera camera = new(
            viewportHeight: Game.ScreenHeight,
            viewportWidth: Game.ScreenWidth
        );
        protected readonly QuickPanel Quick_panel;
        protected ResourcePanel Resource_panel => new(
            Pad, Pad, PanelWidth, PanelHeight, colony_
        );
        protected WeatherPanel Weather_panel => new(
            Pad, Pad + PanelHeight, 
            PanelWidth, PanelHeight, colony_
        );
        protected Rectangle DayCountPanel => new(
            Pad, Game.ScreenHeight - Pad - Game.ScaleInt(30),
            Game.ScaleInt(120), Game.ScaleInt(30)
        );
        protected readonly Button btnNextDay = new(
            Pad, Pad + PanelHeight * 2 + Gap,
            Game.ScaleInt(170), Game.ScaleInt(40),
            "Новый день"
        );

        protected abstract int[] WinCondition { get; set; }
        protected abstract int LoseCondition { get; set; }
        protected bool WinScore { get; set; }
        protected bool LoseScore { get; set; }

        protected Action<int>? OnOpenSpecialist { get; set; }
        protected Action? OnNewDay { get; set; }

        public GameScreen(ScreenManager screenManager, int index)
        {
            levelIndex = index;
            screenManager_ = screenManager;
            World_map = new(seed: levelIndex * 1337 + 42);
            miniMap_ = new MiniMap(
                Game.ScreenWidth - mapW - Pad,
                Pad, mapW, mapH, camera
            );
            Quick_panel = new(
                Game.ScreenWidth - Game.ScaleInt(24) - Game.ScaleInt(200),
                mapH + Pad + Gap,
                Game.ScaleInt(150), Game.ScaleInt(52)
            );
            btnNextDay.BackgroundColor = Assets.Assets.NewDayButtonBackgroundColor;
            btnNextDay.HoveredColor = Assets.Assets.NewDayButtonHoveredColor;
            btnNextDay.TextColor = Assets.Assets.NewDayButtonLabelColor;
            btnNextDay.HoveredTextColor = Assets.Assets.NewDayButtonHoveredLabelColor;
            btnNextDay.BorderColor = Assets.Assets.NewDayButtonHoveredBorderColor;
        }

        public abstract void OnEnter();
        public abstract void OnExit();
        public virtual void Update()
        {
            if (Transition?.IsPlaying == true) return;
            if (btnNextDay.IsLMB_Pressed())
                OnNewDay?.Invoke();
            if (Quick_panel.BtnOpenEngineer.IsLMB_Pressed())
                OnOpenSpecialist?.Invoke(0);
            else if (Quick_panel.BtnOpenScientist.IsLMB_Pressed())
                OnOpenSpecialist?.Invoke(1);
            else if (Quick_panel.BtnOpenLogistics.IsLMB_Pressed())
                OnOpenSpecialist?.Invoke(2);

        }
        public abstract void Draw();
        public abstract void DrawBgImage();
        protected void DrawHUD()
        {
            Resource_panel.Draw();
            Weather_panel.Draw();
            btnNextDay.Draw();
            Quick_panel.Draw();
            DrawDay();
        }

        private void DrawDay()
        {
            DrawAlerts();
            Raylib.DrawRectangleRec(DayCountPanel, Assets.Assets.BackgroundScreenColor);
            Raylib.DrawRectangleLinesEx(DayCountPanel, 1f, Assets.Assets.MiniMapBorderColor);
            string text = $"День {colony_.Day}";
            float ty = DayCountPanel.Y + (DayCountPanel.Height - Assets.Assets.FontMediumSize) / 2f;
            Raylib.DrawTextEx(
            Assets.Assets.FontMedium, text,
                new Vector2(
                    (int)MathF.Round(DayCountPanel.X + PadIn),
                    (int)MathF.Round(ty)
                ),
                Assets.Assets.FontMediumSize, 1, Color.White
            );
        }
            
        private void DrawAlerts()
        {
            List<string> alerts = colony_.GetAlerts();
            if (alerts.Count == 0) return;

            float lineH = Assets.Assets.FontSmallSize + 4;
            float totalH = alerts.Count * lineH + Game.ScaleInt(8);
            float panelW = DayCountPanel.Width + 40 + Game.ScaleInt(20);

            float panelY = DayCountPanel.Y - totalH - Game.ScaleInt(4);
            var panelRect = new Rectangle(DayCountPanel.X, panelY, panelW, totalH);

            Raylib.DrawRectangleRec(panelRect, Assets.Assets.AlertMsgBox);
            Raylib.DrawRectangleLinesEx(panelRect, 0.5f, Assets.Assets.AlertMsgBoxBorder);

            for (int i = 0; i < alerts.Count; i++)
            {
                string msg = alerts[i];

                Color col = msg.Contains("разруш") ? Assets.Assets.AlertBuildingRuinedMsg
                          : msg.Contains("энерги") ? Assets.Assets.AlertBuildingWithoutEnergyMsg
                          : Assets.Assets.AlertBuildingDamagedMsg;

                Raylib.DrawTextEx(Assets.Assets.FontSmall, msg,
                    new Vector2(
                        DayCountPanel.X + Game.ScaleInt(4),
                        panelY + Game.ScaleInt(4) + i * lineH
                    ),
                    Assets.Assets.FontSmallSize, 1, col
                );
            }
        }
    }
}
