using Raylib_cs;
using Space_colony_game.World;
using System.Numerics;

namespace Space_colony_game.UI.Panels
{
    /// <summary>
    /// Панель отображения целей уровня: текущее прогресс-значение и требуемые цели для победы.
    /// </summary>
    public class ObjectivesPanel : Panel
    {
        private readonly Colony colony_;
        private readonly int[] winCondition_;
        private readonly int loseCondition_;

        /// <inheritdoc/>
        public override Rectangle Body { get; set; }

        /// <inheritdoc/>
        public override Color BackgroundColor { get; set; } = new Color(20, 25, 35, 220);

        /// <inheritdoc/>
        public override Color BorderColor { get; set; } = new Color(70, 90, 120, 255);

        /// <inheritdoc/>
        public override float BorderThickness { get; set; } = 2f;

        /// <summary>
        /// Создаёт панель целей.
        /// </summary>
        /// <param name="colony">Колония — источник значений ресурсов.</param>
        /// <param name="winCondition">Массив требований для победы: [еда, металл, люди].</param>
        /// <param name="loseCondition">День, к которому требуется выполнить цели (поражение после достижения).</param>
        /// <param name="body">Прямоугольник панели на экране.</param>
        public ObjectivesPanel(Colony colony, int[] winCondition,
            int loseCondition, Rectangle body)
        {
            colony_ = colony;
            winCondition_ = winCondition;
            loseCondition_ = loseCondition;
            Body = body;
        }

        /// <summary>Рисует панель с заголовком и списком целей с прогрессом.</summary>
        public override void Draw()
        {
            DrawBackground();
            DrawTitle();

            DrawObjective(
                "Накопить еды",
                (int)colony_.Food.Value,
                winCondition_[0], 55
            );

            DrawObjective(
                "Накопить металла",
                (int)colony_.Metal.Value,
                winCondition_[1], 95
            );

            DrawObjective(
                "Разместить колонистов",
                (int)colony_.People.Value,
                winCondition_[2], 135
            );
        }

        private void DrawBackground()
        {
            Raylib.DrawRectangleRec(Body, BackgroundColor);

            Raylib.DrawRectangleLinesEx(
                Body,
                BorderThickness,
                BorderColor
            );
        }

        private void DrawTitle()
        {
            string text = $"Цели: за {loseCondition_} дней";

            Vector2 size = Raylib.MeasureTextEx(
                Assets.Assets.FontMedium,
                text,
                Assets.Assets.FontMediumSize, 1
            );

            Raylib.DrawTextEx(
                Assets.Assets.FontMedium,
                text,
                new Vector2(
                    Body.X + (Body.Width - size.X) / 2,
                    Body.Y + 10
                ), Assets.Assets.FontMediumSize,
                1, Color.White
            );
        }

        private void DrawObjective(string text, int current,
            int required, int offsetY)
        {
            bool completed = current >= required;

            string line = $"{text}: {current}/{required}";

            Color color = completed
                ? new Color(140, 220, 140, 255)
                : new Color(220, 220, 220, 255);

            Vector2 pos = new(
                Body.X + 20,
                Body.Y + offsetY
            );

            Vector2 size = Raylib.MeasureTextEx(
                Assets.Assets.FontSmall,
                line,
                Assets.Assets.FontSmallSize, 1
            );

            Raylib.DrawTextEx(
                Assets.Assets.FontSmall,
                line, pos,
                Assets.Assets.FontSmallSize,
                1, color
            );

            if (completed)
            {
                float lineY = pos.Y + size.Y / 2;

                Raylib.DrawLineEx(
                    new Vector2(pos.X, lineY),
                    new Vector2(pos.X + size.X, lineY),
                    3, Color.Green
                );
            }
        }
    }
}
