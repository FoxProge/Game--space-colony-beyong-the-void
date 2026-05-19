using Raylib_cs;
using System.Numerics;

namespace Space_colony_game.UI
{
    public class Button
    {
        public Rectangle Body { get; set; }
        public string? Label { get; set; }
        public bool IsAvailable { get; set; } = true;
        public Color BackgroundColor { get; set; }
            = Assets.Assets.DefaultButtonColor;
        public Color HoveredColor { get; set; }
            = Assets.Assets.DefaultButtonHoveredColor;
        public Color BorderColor { get; set; }
            = Assets.Assets.DefaultButtonBorderColor;
        public Color HoveredBorderColor { get; set; }
            = Assets.Assets.DefaultButtonHoveredBorderColor;
        public Color TextColor { get; set; }
            = Assets.Assets.DefaultButtonLabelColor;
        public Color HoveredTextColor { get; set; }
            = Assets.Assets.DefaultButtonHoveredLabelColor;
        public Button(
            float x, float y,
            float width, float height,
            string text
        )
        {
            Body = new Rectangle(x, y, width, height);
            Label = text;
        }

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

        public bool IsHovered()
        {
            Vector2 mouse = Raylib.GetMousePosition();
            return Raylib.CheckCollisionPointRec(mouse, Body);
        }

        public bool IsLMB_Pressed()
        {
            return (IsHovered() && Raylib.IsMouseButtonPressed(MouseButton.Left));
        }

        public bool IsRMB_Pressed()
        {
            return (IsHovered() && Raylib.IsMouseButtonPressed(MouseButton.Right));
        }
    }
}
