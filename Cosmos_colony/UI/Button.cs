using Raylib_cs;
using System.Numerics;

namespace Space_colony_game.UI
{
    /// <summary>
    /// Простой UI-кнопка: рисование, проверка наведения и обработки нажатий мышью.
    /// </summary>
    public class Button
    {
        /// <summary>Прямоугольник кнопки на экране.</summary>
        public Rectangle Body { get; set; }

        /// <summary>Текст метки кнопки.</summary>
        public string? Label { get; set; }

        /// <summary>Флаг доступности кнопки (если false — кнопка отрисовывается затемнённой и не реагирует).</summary>
        public bool IsAvailable { get; set; } = true;

        /// <summary>Цвет фона кнопки в обычном состоянии.</summary>
        public Color BackgroundColor { get; set; }
            = Assets.Assets.DefaultButtonColor;

        /// <summary>Цвет фона при наведении курсора.</summary>
        public Color HoveredColor { get; set; }
            = Assets.Assets.DefaultButtonHoveredColor;

        /// <summary>Цвет границы кнопки в обычном состоянии.</summary>
        public Color BorderColor { get; set; }
            = Assets.Assets.DefaultButtonBorderColor;

        /// <summary>Цвет границы при наведении.</summary>
        public Color HoveredBorderColor { get; set; }
            = Assets.Assets.DefaultButtonHoveredBorderColor;

        /// <summary>Цвет текста в обычном состоянии.</summary>
        public Color TextColor { get; set; }
            = Assets.Assets.DefaultButtonLabelColor;

        /// <summary>Цвет текста при наведении.</summary>
        public Color HoveredTextColor { get; set; }
            = Assets.Assets.DefaultButtonHoveredLabelColor;

        /// <summary>Создаёт кнопку с заданными координатами и меткой.</summary>
        public Button(
            float x, float y,
            float width, float height,
            string text
        )
        {
            Body = new Rectangle(x, y, width, height);
            Label = text;
        }

        /// <summary>
        /// Рисует кнопку. Если <paramref name="exitBtn"/> равно true, при наведении используется специальный цвет для кнопки выхода.
        /// </summary>
        /// <param name="exitBtn">Флаг, указывающий, является ли кнопка кнопкой "Выход" (влияет на цвет при наведении).</param>
        public void Draw(bool exitBtn = false)
        {
            Vector2 textSize = Raylib.MeasureTextEx(
                Assets.Assets.FontMedium, Label,
                Assets.Assets.FontMediumSize, 1
            );
            int left = (int)MathF.Round(Body.X);
            int top = (int)MathF.Round(Body.Y);
            int right = (int)MathF.Round(Body.X + Body.Width);
            int bottom = (int)MathF.Round(Body.Y + Body.Height);
            int width = right - left;
            int height = bottom - top;

            float textX = left + (width - textSize.X) / 2f;
            float textY = top + (height - textSize.Y) / 2f;
            
            if(!IsAvailable)
            {
                Raylib.DrawRectangleRec(Body, BackgroundColor);
                Raylib.DrawRectangleLinesEx(Body, 1.5f, BorderColor);
                Raylib.DrawTextEx(
                    Assets.Assets.FontMedium, Label,
                    new Vector2(
                        (int)MathF.Round(textX),
                        (int)MathF.Round(textY)
                    ), Assets.Assets.FontMediumSize, 1,
                    TextColor
                );
                return;
            }

            bool hovered = IsHovered();
            if(hovered)
            {
                if(exitBtn == true)
                    Raylib.DrawRectangleRec(Body, Assets.Assets.ExitButtonHoveredColor);
                else
                    Raylib.DrawRectangleRec(Body, HoveredColor);
                Raylib.DrawRectangleLinesEx(Body, 1.5f, HoveredBorderColor);
            }
            else
            {
                Raylib.DrawRectangleRec(Body, BackgroundColor);
                Raylib.DrawRectangleLinesEx(Body, 1.5f, BorderColor);
            }

            Raylib.DrawTextEx(
                Assets.Assets.FontMedium, Label,
                new Vector2(
                    (int)MathF.Round(textX),
                    (int)MathF.Round(textY)
                ), Assets.Assets.FontMediumSize, 1,
                TextColor
            );
        }

        /// <summary>Проверяет, наведён ли курсор мыши на область кнопки.</summary>
        public bool IsHovered()
        {
            Vector2 mouse = Raylib.GetMousePosition();
            return Raylib.CheckCollisionPointRec(mouse, Body);
        }

        /// <summary>Проверяет, была ли нажата левая кнопка мыши по кнопке в данном кадре.</summary>
        public bool IsLMB_Pressed()
        {
            return (IsHovered() && Raylib.IsMouseButtonPressed(MouseButton.Left));
        }

        /// <summary>Проверяет, была ли нажата правая кнопка мыши по кнопке в данном кадре.</summary>
        public bool IsRMB_Pressed()
        {
            return (IsHovered() && Raylib.IsMouseButtonPressed(MouseButton.Right));
        }
    }
}
