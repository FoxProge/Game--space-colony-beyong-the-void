using Raylib_cs;
using Space_colony_game.Systems;
using System.Numerics;

namespace Space_colony_game.World
{
    public enum TileType
    {
        Ground,
        Rock,
        Sand, 
        Ice,
        Crater
    }

    public class Tile
    {
        public TileType Type { get; init; }
        public int TextureId { get; init; } = 0;
        public bool Buildable => Type is TileType.Ground or TileType.Sand;
    }

    public class WorldMap
    {
        public static readonly int Columns = 50;
        public static readonly int Rows = 50;
        public static readonly int TileSize = 64;

        public static int WorldWidth { get; private set; } =  Columns * TileSize;
        public static int WorldHeight { get; private set; } =  Columns * TileSize;

        public static readonly Tile[,] Tiles = new Tile[Columns, Rows];

        public WorldMap(int seed = 13)
        {
            GenerateMap(seed);
        }

        public Tile GetTile(int col, int row)
        {
            if (col < 0 || col >= Columns || row < 0 || row >= Rows)
                return new Tile { Type = TileType.Rock };
            return Tiles[col, row];
        }

        public void Draw(Camera camera)
        {
            int colFrom = Math.Max(0, (int)(camera.Position.X / TileSize));
            int colTo = Math.Min(
                Columns,
                (int)((camera.Position.X + camera.ViewportWidth / camera.Zoom) / TileSize) + 2
            );

            int rowFrom = Math.Max(0, (int)(camera.Position.Y / TileSize));
            int rowTo = Math.Min(
                Rows,
                (int)((camera.Position.Y + camera.ViewportHeight / camera.Zoom) / TileSize) + 2
            );

            for (int col = colFrom; col < colTo; col++)
            {
                for (int row = rowFrom; row < rowTo; row++)
                {
                    Tile tile = Tiles[col, row];

                    float wx = col * TileSize;
                    float wy = row * TileSize;

                    var screen = camera.WorldToScreen(new Vector2(wx, wy));

                    float fw = TileSize * camera.Zoom;
                    float fh = TileSize * camera.Zoom;

                    int left = (int)MathF.Round(screen.X);
                    int top = (int)MathF.Round(screen.Y);
                    int right = (int)MathF.Round(screen.X + fw);
                    int bottom = (int)MathF.Round(screen.Y + fh);

                    int sw = right - left;
                    int sh = bottom - top;

                    DrawTile(tile, left, top, sw, sh);
                }
            }
        }

        private void GenerateMap(int seed)
        {
            var rng = new Random(seed);

            for (int col = 0; col < Columns; col++)
            {
                for (int row = 0; row < Rows; row++)
                {
                    int roll = rng.Next(100);
                    TileType type = roll switch
                    {
                        < 65 => TileType.Ground,
                        < 83 => TileType.Sand,
                        < 96 => TileType.Rock,
                        < 99 => TileType.Crater,
                        _ => TileType.Rock,
                    };

                    int texIdx = type switch
                    {
                        TileType.Ground => rng.Next(3),
                        TileType.Rock => rng.Next(2),
                        TileType.Sand => rng.Next(3),
                        _ => 0,
                    };

                    Tiles[col, row] = new Tile { Type = type, TextureId = texIdx };
                }
            }

            for (int col = 22; col <= 27; col++)
                for (int row = 22; row <= 27; row++)
                    Tiles[col, row] = new Tile
                    {
                        Type = TileType.Ground,
                        TextureId = new Random(col * 100 + row).Next(3)
                    };
        }

        private static void DrawTile(Tile tile, int x, int y, int w, int h)
        {
            var dest = new Rectangle(x, y, w, h);

            switch (tile.Type)
            {
                case TileType.Ground:
                    DrawWithFallback(Assets.Assets.TileGround, tile.TextureId, dest, Assets.Assets.ColorGround);
                    break;
                case TileType.Rock:
                    DrawWithFallback(Assets.Assets.TileRock, tile.TextureId, dest, Assets.Assets.ColorRock);
                    break;
                case TileType.Sand:
                    DrawWithFallback(Assets.Assets.TileSand, tile.TextureId, dest, Assets.Assets.ColorSand);
                    break;
                case TileType.Crater:
                    if (Assets.Assets.TileCrater.Id != 0)
                        Raylib.DrawTexturePro(Assets.Assets.TileCrater,
                            new Rectangle(0, 0, Assets.Assets.TileCrater.Width, Assets.Assets.TileCrater.Height),
                            dest, Vector2.Zero, 0f, Color.White);
                    else
                        Raylib.DrawRectangleRec(dest, Assets.Assets.ColorCrater);
                    break;
                case TileType.Ice:
                    Raylib.DrawRectangleRec(dest, Assets.Assets.ColorIce);
                    break;
            }

            if (w >= 16)
                Raylib.DrawRectangleLinesEx(dest, 0.5f, Assets.Assets.ColorGrid);

        }

        private static void DrawWithFallback(
            Texture2D[] textures, int idx, Rectangle dest, Color fallback)
        {
            if (textures.Length > 0)
            {
                var tex = textures[idx % textures.Length];
                if (tex.Id != 0)
                {
                    Raylib.DrawTexturePro(tex,
                        new Rectangle(0, 0, tex.Width, tex.Height),
                        dest, Vector2.Zero, 0f, Color.White);
                    return;
                }
            }
            Raylib.DrawRectangleRec(dest, fallback);
        }

        public static Color TileColor(TileType type) => type switch
        {
            TileType.Ground => Assets.Assets.ColorGround,
            TileType.Rock => Assets.Assets.ColorRock,
            TileType.Sand => Assets.Assets.ColorSand,
            TileType.Ice => Assets.Assets.ColorIce,
            TileType.Crater => Assets.Assets.ColorCrater,
            _ => Assets.Assets.ColorGround,
        };
    }
}
