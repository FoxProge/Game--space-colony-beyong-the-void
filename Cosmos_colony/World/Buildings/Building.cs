using Raylib_cs;
using Space_colony_game.Systems;
using System.Numerics;

namespace Space_colony_game.World.Buildings
{
    public abstract class Building : Entity 
    {
        public BuildingType? Type { get; init; } = null;
        public int Health { get; set; } = 100;
        public virtual bool CanBeDestroyed => true;

        public int SizeX => Type.SizeX;
        public int SizeY => Type.SizeY;

        public Rectangle WorldRec => new(
            Col * WorldMap.TileSize,
            Row * WorldMap.TileSize,
            SizeX * WorldMap.TileSize,
            SizeY * WorldMap.TileSize
        );

        public override void Draw(Camera camera)
        {
            float wx = Col * WorldMap.TileSize;
            float wy = Row * WorldMap.TileSize;

            Vector2 screen = camera.WorldToScreen(new Vector2(wx, wy));

            float fw = SizeX * WorldMap.TileSize * camera.Zoom;
            float fh = SizeY * WorldMap.TileSize * camera.Zoom;

            int left = (int)MathF.Round(screen.X);
            int top = (int)MathF.Round(screen.Y);
            int right = (int)MathF.Round(screen.X + fw);
            int bottom = (int)MathF.Round(screen.Y + fh);

            int sw = right - left;
            int sh = bottom - top;

            DrawBody(left, top, sw, sh);

            if (this is PoweredBuilding pb)
                DrawStatusIcon(pb, left, top, sw, sh);
        }

        private static void DrawStatusIcon(PoweredBuilding pb, int x, int y, int w, int h)
        {
            if (!pb.IsBroken && pb.HasPower) return;

            int size = (int)Math.Max(10, Math.Min(w, h) * 0.28f);
            int px = x + w - size - 3;
            int py = y + 3;

            Color color = pb.IsBroken
                ? new Color(220, 50, 50, 220)
                : new Color(220, 180, 30, 220);

            Raylib.DrawTriangle(
                new Vector2(px + size / 2f, py),
                new Vector2(px, py + size),
                new Vector2(px + size, py + size),
                color);

            if (size >= 14)
                Raylib.DrawTextEx(Assets.Assets.FontSmall, "!",
                    new Vector2(px + size / 2f - 3, py + 2),
                    Assets.Assets.FontSmallSize * 0.75f, 1, Color.White);
        }

        protected static void DrawTextureOrFallback(
            Texture2D? tex, int x, int y, int w, int h, Color fallback)
        {
            if (tex.HasValue && tex.Value.Id != 0)
                Raylib.DrawTexturePro(tex.Value,
                    new Rectangle(0, 0, tex.Value.Width, tex.Value.Height),
                    new Rectangle(x, y, w, h),
                    Vector2.Zero, 0f, Color.White);
            else
                Raylib.DrawRectangle(x, y, w, h, fallback);
        }

        public virtual void DrawBody(int x, int y, int w, int h)
        {
            var rect = new Rectangle(x, y, w, h);

            Raylib.DrawRectangleRec(rect, Type.Color);
            Raylib.DrawRectangleLinesEx(
                rect, 1.5f,
                new Color(0, 0, 0, 100)
            );

            if (w > 20 && h > 14)
            {
                var sz = Raylib.MeasureTextEx(
                    Assets.Assets.FontSmall, Type.Abbr,
                    Assets.Assets.FontSmallSize, 1
                );

                float ftx = x + (w - sz.X) / 2f;
                float fty = y + (h - sz.Y) / 2f;

                int tx = (int)MathF.Round(ftx);
                int ty = (int)MathF.Round(fty);

                Raylib.DrawTextEx(
                    Assets.Assets.FontSmall, Type.Abbr,
                    new Vector2(tx, ty),
                    Assets.Assets.FontSmallSize, 1,
                    Type.AbbrevColor
                );
            }
        }
    }
}
