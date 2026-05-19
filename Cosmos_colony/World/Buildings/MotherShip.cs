using Raylib_cs;
using System.Numerics;

namespace Space_colony_game.World.Buildings
{
    public class MotherShip : Building
    {
        public const int Size = 4;
        public override bool CanBeDestroyed => false;

        public Action? OnrightClick { get; set; }

        //анимация пульсации рамки
        private float _pulseTimer = 0f;

        public MotherShip(int col, int row, BuildingType type)
        {
            _ = col; _ = row; _ = type;
        }

        public static MotherShip Create(int col, int row, BuildingType type)
        {
            return new MotherShip(col, row, type)
            {
                Type = type,
                Col = col,
                Row = row,
                Id = 0
            };
        }

        public override void OnDayPassed() { }

        public override void DrawBody(int x, int y, int w, int h)
        {
            if (Assets.Assets.TextureSpaceShip is { } tex && tex.Id != 0)
            {
                Raylib.DrawTexturePro(tex,
                    new Rectangle(0, 0, tex.Width, tex.Height),
                    new Rectangle(x, y, w, h),
                    Vector2.Zero, 0f, Color.White
                );
                return;
            }

            float fx = MathF.Round(x);
            float fy = MathF.Round(y);
            float fw = MathF.Round(w);
            float fh = MathF.Round(h);

            int ix = (int)fx;
            int iy = (int)fy;
            int iw = (int)fw;
            int ih = (int)fh;

            Raylib.DrawRectangle(ix, iy, iw, ih, new Color(20, 40, 80, 255));

            int pad = Math.Max(4, iw / 8);

            int ix2 = ix + pad;
            int iy2 = iy + pad;
            int iw2 = iw - pad * 2;
            int ih2 = ih - pad * 2;

            Raylib.DrawRectangle(
                ix2, iy2, iw2, ih2,
                new Color(30, 60, 120, 255));

            int cx = ix + iw / 2;
            int cy = iy + ih / 2;
            int r = Math.Min(iw, ih) / 5;

            Raylib.DrawCircle(cx, cy, r, new Color(80, 140, 220, 255));
            Raylib.DrawCircleLines(cx, cy, r, new Color(120, 180, 255, 200));

            _pulseTimer += Raylib.GetFrameTime();
            float pulse = (float)(Math.Sin(_pulseTimer * 2.0) * 0.5 + 0.5);
            byte alpha = (byte)(130 + pulse * 125);

            Raylib.DrawRectangleLinesEx(
                new Rectangle(fx, fy, fw, fh),
                2f,
                new Color((byte)80, (byte)140, (byte)220, alpha)
            );

            if (iw > 30)
            {
                Vector2 sz = Raylib.MeasureTextEx(
                    Assets.Assets.FontSmall, "МК",
                    Assets.Assets.FontSmallSize, 1
                );

                float tx = MathF.Round(fx + (fw - sz.X) * 0.5f);
                float ty = MathF.Round(fy + fh - sz.Y - 6);

                Raylib.DrawTextEx(
                    Assets.Assets.FontSmall, "МК",
                    new Vector2(tx, ty),
                    Assets.Assets.FontSmallSize, 1,
                    new Color(180, 210, 255, 255)
                );
            }
        }
    }
}
