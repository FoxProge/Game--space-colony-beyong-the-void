using Raylib_cs;
using Space_colony_game.Core;
using Space_colony_game.Systems;
using Space_colony_game.World;
using System.Numerics;

namespace Space_colony_game.UI.Panels
{
    /// <summary>
    /// Панель прогноза погоды — рисует прогноз на ближайшие дни и обозначает опасные дни цветом.
    /// </summary>
    public class WeatherPanel : Panel
    {
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
        /// Возвращает строковый значок для данного типа погоды.
        /// Используется для компактного отображения иконок в панели.
        /// </summary>
        /// <param name="d">День прогноза.</param>
        /// <returns>Короткая строка-иконка.</returns>
        private static string WeatherIcon(WeatherDay d) => d.Type switch
        {
            WeatherType.Sunny => "*",
            WeatherType.Cloudy => "~",
            WeatherType.Storm => "!",
            WeatherType.Blizzard => "!!",
            _ => "??"
        };

        /// <summary>
        /// Создаёт панель прогноза погоды.
        /// </summary>
        /// <param name="x">Позиция X панели (пиксели).</param>
        /// <param name="y">Позиция Y панели (пиксели).</param>
        /// <param name="width">Ширина панели (пиксели).</param>
        /// <param name="height">Высота панели (пиксели).</param>
        /// <param name="colony">Объект колонии, откуда берётся прогноз погоды.</param>
        public WeatherPanel(float x, float y, float width, float height, Colony colony)
        {
            colony_ = colony;
            Body = new(x, y, width, height);
            BackgroundColor = Assets.Assets.BackgroundScreenColor;
            BorderColor = Assets.Assets.MiniMapBorderColor;
            BorderThickness = 1f;
        }

        /// <summary>
        /// Рисует панель погоды: фон, разделители и текст прогноза для ближайших дней.
        /// Подкрашивает текущий день отдельно и опасные дни — цветом предупреждения.
        /// </summary>
        public override void Draw()
        {
            Raylib.DrawRectangleRec(Body, BackgroundColor);
            Raylib.DrawRectangleLinesEx(Body, BorderThickness, BorderColor);

            var forecast = colony_.WeatherSys.Forecast;
            int count = Math.Min(7, forecast.Count);
            float cellW = Game.ScaleInt(500) / (float)count;

            for (int i = 0; i < count; i++)
            {
                var day = forecast[i];

                float cx = Body.X + i * cellW;
                float cy = Body.Y;

                Color color = i == 0 ? Color.SkyBlue : (day.IsDangerous ? Color.Orange : Color.LightGray);

                string cell = $"{WeatherIcon(day)}{day.Temp:+0;-0}°";

                float ty = cy + ((int)(Game.ScreenHeight * 0.052f) - Assets.Assets.FontMediumSize) / 2f;

                Raylib.DrawTextEx(
                    Assets.Assets.FontMedium, cell,
                    new Vector2(
                        (int)MathF.Round(cx + Game.ScaleInt(10)),
                        (int)MathF.Round(ty)
                    ),
                    Assets.Assets.FontMediumSize, 1, color
                );

                if (i < count - 1)
                {
                    int x = (int)MathF.Round(cx + cellW);

                    Raylib.DrawLineV(
                        new Vector2(x, (int)MathF.Round(cy + Game.ScaleF(4))),
                        new Vector2(x, (int)MathF.Round(cy + (int)(Game.ScreenHeight * 0.052f) - Game.ScaleF(4))),
                        BorderColor
                    );
                }
            }
        }
    }
}
