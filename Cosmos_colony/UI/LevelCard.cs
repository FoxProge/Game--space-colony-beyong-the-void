using Raylib_cs;
using System.Numerics;

namespace Space_colony_game.UI
{
    public class LevelCard
    {
        public Rectangle Body {  get; set; }
        public Color BackgroundColor { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public bool IsSelected { get; set; } = false;

        public LevelCard(
            float x, float y,
            float width, float height,
            Color backColor, string text,
            string descText
        )
        {
            Body = new(x, y, width, height);
            BackgroundColor = backColor;
            Label = text;
            Description = descText;
        }

        public void Draw()
        {
            int left = (int)MathF.Round(Body.X);
            int top = (int)MathF.Round(Body.Y);
            int right = (int)MathF.Round(Body.X + Body.Width);
            int bottom = (int)MathF.Round(Body.Y + Body.Height);
            int width = right - left;
            int height = bottom - top;

            Vector2 labelSize = Raylib.MeasureTextEx(
                Assets.Assets.FontMedium,
                Label, 28, 1
            );

            Color bodyBorderColor = IsSelected ? Color.White : Color.Gray;
            float bodyBorderThick = IsSelected ? 3f : 1f;
            Raylib.DrawRectangleRec(Body, BackgroundColor);
            Raylib.DrawRectangleLinesEx(Body, bodyBorderThick, bodyBorderColor);

            float fLabelX = left + (width - labelSize.X) / 2f;
            int labelX = (int)MathF.Round(fLabelX);
            int labelY = bottom - 106;
            Raylib.DrawTextEx(
                Assets.Assets.FontMedium, Label,
                new Vector2(labelX, labelY), 28, 1,
                Color.White
            );

            int descX = left + 12;
            int descY = bottom - 78;
            Raylib.DrawTextEx(
                Assets.Assets.FontMedium, Description,
                new Vector2(descX, descY),
                Assets.Assets.FontMediumSize, 1,
                Color.LightGray
            );

            if(IsSelected)
            {
                const string tag = "ВЫБРАНО";
                Vector2 tagSize = Raylib.MeasureTextEx(Assets.Assets.FontMedium, tag, 16, 1);
                float fTagX = left + (width - tagSize.X) / 2f;
                int tagX = (int)MathF.Round(fTagX);
                int tagY = top + 10;
                Raylib.DrawTextEx(
                    Assets.Assets.FontMedium, tag,
                    new Vector2(tagX, tagY), 16, 1,
                    Color.Yellow
                );
            }
        }

        public bool IsHovered()
        {
            Vector2 mouse = Raylib.GetMousePosition();
            return Raylib.CheckCollisionPointRec(mouse, Body);
        }

        public bool IsPressed()
        {
            return (IsHovered() && Raylib.IsMouseButtonPressed(MouseButton.Left));
        }
    }
}
