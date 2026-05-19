using System;
using System.Collections.Generic;
using Raylib_cs;
using Space_colony_game.Core;
using Space_colony_game.Systems;
using Space_colony_game.UI;
using Space_colony_game.UI.Panels;
using Space_colony_game.World;
using System.Numerics;

namespace Space_colony_game.Screens
{
    /// <summary>
    /// Базовый экран игры, содержит общую логику HUD, карту мира и управление переходами между игровыми экранами уровня.
    /// </summary>
    public abstract class GameScreen : IScreen
    {
        /// <summary>Соотношение ширины мини-карты к ширине экрана.</summary>
        private const float MinimapScreenRatio = 0.22f;

        /// <summary>Ширина мини-карты, вычисляемая от текущей ширины экрана.</summary>
        private float mapW => Game.ScreenWidth * MinimapScreenRatio;

        /// <summary>Высота мини-карты (пропорция от ширины мини-карты).</summary>
        private float mapH => mapW * 0.66f;

        /// <summary>Менеджер экранов, использующийся для управления переходами.</summary>
        protected readonly ScreenManager screenManager_;

        /// <summary>Стандартные отступы и размеры, масштабируемые в зависимости от настроек игры.</summary>
        protected static int Pad => Game.ScaleInt(12);
        protected static int PadIn => Game.ScaleInt(10);
        protected static int Gap => Game.ScaleInt(4);
        protected static int PanelWidth => Game.ScaleInt(500);
        protected static int PanelHeight => (int)(Game.ScreenHeight * 0.052f);

        /// <summary>Индекс уровня, ассоциированный с этим экраном.</summary>
        public int levelIndex { get; private set; }

        /// <summary>Экземпляр колонии, содержащий состояние игровой логики (ресурсы, дни и т. п.).</summary>
        protected readonly Colony colony_ = new();

        /// <summary>Отвечает за анимации перехода дня.</summary>
        protected readonly DayTransition Transition = new();

        /// <summary>Карта мира текущего уровня.</summary>
        protected readonly WorldMap World_map;

        /// <summary>Мини-карта для отображения обзора мира.</summary>
        protected readonly MiniMap miniMap_;

        /// <summary>Камера, управляющая смещением и масштабом игрового мира для отрисовки.</summary>
        protected readonly Camera camera = new(
            viewportHeight: Game.ScreenHeight,
            viewportWidth: Game.ScreenWidth
        );

        /// <summary>Быстрая панель с кнопками доступа к специалистам и другим действиям.</summary>
        protected readonly QuickPanel Quick_panel;

        /// <summary>
        /// Панель ресурсов. Каждый вызов создаёт экземпляр панели, привязанной к текущей колонии и позиционированию HUD.
        /// </summary>
        protected ResourcePanel Resource_panel => new(
            Pad, Pad, PanelWidth, PanelHeight, colony_
        );

        /// <summary>
        /// Панель погоды. Каждый вызов создаёт экземпляр панели, привязанной к текущей колонии и позиционированию HUD.
        /// </summary>
        protected WeatherPanel Weather_panel => new(
            Pad, Pad + PanelHeight, 
            PanelWidth, PanelHeight, colony_
        );

        /// <summary>Прямоугольник панели, показывающей номер дня внизу экрана.</summary>
        protected Rectangle DayCountPanel => new(
            Pad, Game.ScreenHeight - Pad - Game.ScaleInt(30),
            Game.ScaleInt(120), Game.ScaleInt(30)
        );

        /// <summary>Кнопка перехода к следующему дню.</summary>
        protected readonly Button btnNextDay = new(
            Pad, Pad + PanelHeight * 2 + Gap,
            Game.ScaleInt(170), Game.ScaleInt(40),
            "Новый день"
        );

        /// <summary>Условия победы (массив значений, проверяемых для определения выигрыша).</summary>
        protected abstract int[] WinCondition { get; set; }

        /// <summary>Пороговое значение, при достижении которого считается проигрыш.</summary>
        protected abstract int LoseCondition { get; set; }

        /// <summary>Флаг, указывающий, что достигнуто условие победы.</summary>
        protected bool WinScore { get; set; }

        /// <summary>Флаг, указывающий, что достигнуто условие поражения.</summary>
        protected bool LoseScore { get; set; }

        /// <summary>
        /// Делегат, вызываемый при открытии окна специалиста.
        /// Параметр — индекс специалиста (например, 0 — инженер, 1 — учёный, 2 — логистика).
        /// </summary>
        protected Action<int>? OnOpenSpecialist { get; set; }

        /// <summary>Делегат, вызываемый при наступлении нового дня.</summary>
        protected Action? OnNewDay { get; set; }

        /// <summary>
        /// Создаёт экземпляр игрового экрана для указанного уровня.
        /// </summary>
        /// <param name="screenManager">Менеджер экранов, использующийся для управления переходами.</param>
        /// <param name="index">Индекс уровня (seed для генератора мира).</param>
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

        /// <summary>Вызывается при входе на экран (должен быть реализован в подклассах).</summary>
        public abstract void OnEnter();

        /// <summary>Вызывается при выходе с экрана (должен быть реализован в подклассах).</summary>
        public abstract void OnExit();

        /// <summary>
        /// Обновляет состояние экрана: обработка входа пользователя, планирование нового дня и переключений.
        /// Если воспроизводится переход дня (<see cref="Transition"/>), другой ввод игнорируется.
        /// </summary>
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

        /// <summary>Рисует экран (фон, мир и HUD) — реализация в подклассах.</summary>
        public abstract void Draw();

        /// <summary>Рисует фоновое изображение или картинку уровня.</summary>
        public abstract void DrawBgImage();

        /// <summary>Рисует HUD: панели ресурсов, погоды, кнопку нового дня, быстрые кнопки и индикатор дня.</summary>
        protected void DrawHUD()
        {
            Resource_panel.Draw();
            Weather_panel.Draw();
            btnNextDay.Draw();
            Quick_panel.Draw();
            DrawDay();
        }

        /// <summary>Рисует индикатор текущего дня и связанные элементы HUD.</summary>
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
            
        /// <summary>
        /// Рисует список предупреждений/уведомлений колонии над индикатором дня.
        /// Сообщения берутся через <see cref="Colony.GetAlerts"/>.
        /// </summary>
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
