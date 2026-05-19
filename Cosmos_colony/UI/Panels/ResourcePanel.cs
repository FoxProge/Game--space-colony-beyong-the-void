using Raylib_cs;
using Space_colony_game.Core;
using Space_colony_game.World;
using System.Numerics;


namespace Space_colony_game.UI.Panels
{
    /// <summary>
    /// Панель отображения ключевых ресурсов колонии (еда, металл, энергия, люди).
    /// Рисует иконки, текущее значение и дельту за ход.
    /// </summary>
    public class ResourcePanel : Panel
    {
        /// <summary>Внутренний отступ слева для элементов.</summary>
        private int PadIn => Game.ScaleInt(10);

        /// <summary>Высота панели в пикселях (зависит от размера экрана).</summary>
        private int PanelHeight => (int)(Game.ScreenHeight * 0.052f);

        /// <inheritdoc/>
        public override Rectangle Body { get; set; }

        /// <inheritdoc/>
        public override Color BackgroundColor { get; set; }

        /// <inheritdoc/>
        public override Color BorderColor { get; set; }

        /// <inheritdoc/>
        public override float BorderThickness { get; set; }

        private readonly Colony colony_;

        /// <summary>
        /// Вспомогательная структура описывает иконку ресурса и функции получения значения и дельты.
        /// </summary>
        private readonly record struct ResInfo(
            Texture2D Icon, Func<Colony, float> Val, Func<Colony, float> Delta
        );

        /// <summary>Массив описателей ресурсов в порядке отображения.</summary>
        private static readonly ResInfo[] Res =
        {
            new(Assets.Assets.FoodIcon, c => c.Food.Value,   c => c.Food.Delta),
            new(Assets.Assets.MetalIcon, c => c.Metal.Value,  c => c.Metal.Delta),
            new(Assets.Assets.EnergyIcon, c => c.Energy.Value, c => c.Energy.Delta),
            new(Assets.Assets.PeopleIcon, c => c.People.Value, c => c.People.Delta),
        };

        /// <summary>
        /// Создаёт панель ресурсов.
        /// </summary>
        /// <param name="x">Позиция X панели (пиксели).</param>
        /// <param name="y">Позиция Y панели (пиксели).</param>
        /// <param name="width">Ширина панели (пиксели).</param>
        /// <param name="height">Высота панели (пиксели).</param>
        /// <param name="colony">Объект колонии, источник данных ресурсов.</param>
        public ResourcePanel(float x, float y, float width, float height, Colony colony)
        {
            colony_ = colony;
            Body = new(x, y, width, height);
            BackgroundColor = Assets.Assets.BackgroundScreenColor;
            BorderColor = Assets.Assets.MiniMapBorderColor;
            BorderThickness = 1f;
        }

        /// <summary>Рисует панель: фон, разделители, иконки, значения и дельты ресурсов.</summary>
        public override void Draw()
        {
            Raylib.DrawRectangleRec(Body, BackgroundColor);
            Raylib.DrawRectangleLinesEx(Body, BorderThickness, BorderColor);

            float cellW = Body.Width / Res.Length;

            const float iconSize = 32f;

            for (int i = 0; i < Res.Length; i++)
            {
                float cx = Body.X + i * cellW;
                float cy = Body.Y + PanelHeight / 2f;

                float val = Res[i].Val(colony_);
                float delta = Res[i].Delta(colony_);

                float iconX = cx + PadIn;
                float iconY = cy - iconSize / 2f;

                Raylib.DrawTexturePro(
                    Res[i].Icon,
                    new Rectangle(0, 0, Res[i].Icon.Width, Res[i].Icon.Height),
                    new Rectangle(iconX, iconY, iconSize, iconSize),
                    Vector2.Zero,
                    0f,
                    Color.White
                );

                string valStr = ((int)val).ToString();

                Vector2 valSize = Raylib.MeasureTextEx(
                    Assets.Assets.FontMedium,
                    valStr,
                    Assets.Assets.FontMediumSize,
                    1
                );

                float valueX = iconX + iconSize + Game.ScaleF(8);
                float valueY = cy - valSize.Y / 2f;

                Raylib.DrawTextEx(
                    Assets.Assets.FontMedium,
                    valStr,
                    new Vector2(valueX, valueY),
                    Assets.Assets.FontMediumSize,
                    1,
                    Color.White
                );

                if (delta != 0)
                {
                    string dStr = $"{(delta > 0 ? "+" : "")}{(int)delta}";

                    Vector2 deltaSize = Raylib.MeasureTextEx(
                        Assets.Assets.FontSmall,
                        dStr,
                        Assets.Assets.FontSmallSize,
                        1
                    );

                    float deltaX = valueX + valSize.X + Game.ScaleF(6);
                    float deltaY = cy - deltaSize.Y / 2f + Game.ScaleF(1);

                    Raylib.DrawTextEx(
                        Assets.Assets.FontSmall,
                        dStr,
                        new Vector2(deltaX, deltaY),
                        Assets.Assets.FontSmallSize,
                        1,
                        delta > 0 ? Color.Green : Color.Red
                    );
                }

                if (i < Res.Length - 1)
                {
                    int x = (int)MathF.Round(cx + cellW);

                    Raylib.DrawLineV(
                        new Vector2(x, Body.Y + Game.ScaleF(5)),
                        new Vector2(x, Body.Y + PanelHeight - Game.ScaleF(5)),
                        BorderColor
                    );
                }
            }
        }
    }
}
